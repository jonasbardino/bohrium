#include <fstream>
#include <sstream>
#include <set>
#include "dag.hpp"
#include "utils.hpp"

using namespace std;
using namespace boost;
namespace bohrium{
namespace core {

const char Dag::TAG[] = "Dag";

bool Dag::fusable(bh_instruction* prev, bh_instruction* cur)
{
    bool compatible = false;

    switch(prev->opcode) {               // Verify the opcode
        case BH_RANDOM:
        case BH_RANGE:
        case BH_ADD_ACCUMULATE:
        case BH_MULTIPLY_ACCUMULATE:
        case BH_ADD_REDUCE:
        case BH_MULTIPLY_REDUCE:
        case BH_MINIMUM_REDUCE:
        case BH_MAXIMUM_REDUCE:
        case BH_LOGICAL_AND_REDUCE:
        case BH_BITWISE_AND_REDUCE:
        case BH_LOGICAL_OR_REDUCE:
        case BH_BITWISE_OR_REDUCE:
        case BH_LOGICAL_XOR_REDUCE:
        case BH_BITWISE_XOR_REDUCE:
            return compatible;
        default:
            break;
    }

    // Check shapes and dependencies

    compatible = true;
    return compatible;
}

/**
 *  Construct a list of subgraphs...
 */
void Dag::partition(void)
{
    DEBUG(TAG,"partition(...)");

    int64_t graph_idx=0;

    subgraphs_.push_back(&(graph_.create_subgraph()));    // Create the first subgraph
    bh_instruction* prev = &(instr_[0]);
    add_vertex(0, *(subgraphs_[graph_idx]));            // Add the first instruction

    for(int64_t idx=1; idx < ninstr_; ++idx) {    // Then look at the remaining
        bh_instruction* cur = &(instr_[idx]);

        if (!fusable(prev, cur)) {
            subgraphs_.push_back(&(graph_.create_subgraph()));
            ++graph_idx;
        }
        add_vertex(idx, *(subgraphs_[graph_idx]));

        prev = cur;
    }

    DEBUG(TAG,"partition(...);");
}

void Dag::array_deps(void)
{
    DEBUG(TAG,"array_deps(...)");
    //
    // Find dependencies on array operations
    for(int64_t idx=0; idx < ninstr_; ++idx) {
        // The instruction to find data-dependencies for
        tac_t& tac = tacs_[idx];

        // Ignore sys-ops
        if ((tac.op == SYSTEM) || (NOOP == tac.op)) {
            DEBUG(TAG, "Ignoring system...");
            continue;
        }

        // Bases associated with the instruction
        bh_base* output = symbol_table_[tac.out].base;

        bool found = false;
        for(int64_t other=idx+1; (other<ninstr_) && (!found); ++other) {
            tac_t& other_tac = tacs_[other];

            // Ignore sys and noops
            if ((other_tac.op == SYSTEM) || (NOOP == other_tac.op)) {
                DEBUG(TAG, "Ignoring system...inside...");
                continue;
            }

            // Search operands of other instruction
            switch(tac_noperands(other_tac)) {
                case 3:
                    DEBUG(TAG, "Comparing" << symbol_table_[other_tac.in2].base << " == " << output);
                    if (symbol_table_[other_tac.in2].layout != CONSTANT) {
                        if (symbol_table_[other_tac.in2].base == output) {
                            found = true;
                            add_edge(idx, other, graph_);
                            break;
                        }
                    }
                case 2:
                    DEBUG(TAG, "Comparing" << symbol_table_[other_tac.in1].base << " == " << output);
                    if (symbol_table_[other_tac.in1].layout != CONSTANT) {
                        if (symbol_table_[other_tac.in1].base == output) {
                            found = true;
                            add_edge(idx, other, graph_);
                            break;
                        }
                    }
                case 1:
                    DEBUG(TAG, "Comparing" << symbol_table_[other_tac.out].base << " == " << output);
                    if (symbol_table_[other_tac.out].base == output) {
                        found = true;
                        add_edge(idx, other, graph_);
                        break;
                    }
                default:
                    break;
            }
        }
        DEBUG(TAG, "Found=" << found << ".");
    }
    DEBUG(TAG,"array_deps(...);");
}

void Dag::system_deps(void)
{
    DEBUG(TAG,"system_deps(...)");
    //
    // Find dependencies on system operations
    for(int64_t idx=ninstr_-1; idx>=0; --idx) {

        // The instruction to find data-dependencies for
        bh_instruction* instr = &instr_[idx];

        if (!((instr->opcode == BH_FREE)     ||  \
              (instr->opcode == BH_DISCARD)  ||  \
              (instr->opcode == BH_SYNC)     ||  \
              (instr->opcode == BH_NONE))) {
            continue;
        }

        // Bases associated with the instruction
        set<bh_base*> inputs;
        bh_base* output = NULL;
        for(int64_t op_idx=0; op_idx<bh_operands(instr->opcode); ++op_idx) {

            if (0 == op_idx) {
                output = instr->operand[op_idx].base;
            } else {
                if (!bh_is_constant(&instr->operand[op_idx])) {
                    inputs.insert(instr->operand[op_idx].base);
                }
            }
        }

        bool found = false;
        for(int64_t other=idx-1; (other>=0) && (!found); --other) {
            bh_instruction* other_instr = &instr_[other];

            // Search operands of other instruction
            int64_t noperands = bh_operands(other_instr->opcode);
            for(int64_t op_idx=0; op_idx<noperands; ++op_idx) {
                // Array operations dependent on the output
                bh_view* other_op   = &other_instr->operand[op_idx];
                bh_base* other_base = other_op->base;

                if (other_base == output) {
                    found = true;
                    add_edge(other, idx, graph_);
                    break;
                }
            }
        }
    }
    DEBUG(TAG,"system_deps(...);");
}

}}
