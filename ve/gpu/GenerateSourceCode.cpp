/*
 * Copyright 2012 Troels Blum <troels@blum.dk>
 *
 * This file is part of cphVB <http://code.google.com/p/cphvb/>.
 *
 * cphVB is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * cphVB is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with cphVB. If not, see <http://www.gnu.org/licenses/>.
 */

#include <cassert>
#include <stdexcept>
#include "GenerateSourceCode.hpp"

void generateGIDSource(std::vector<cphvb_index> shape, std::ostream& source)
{
    size_t ndim = shape.size();
    assert(ndim > 0);    
    if (ndim > 2)
    {
        source << "\tconst size_t gidz = get_global_id(2);\n";
        source << "\tif (gidz >= " << shape[ndim-3] << ")\n\t\treturn;\n";
    }
    if (ndim > 1)
    {
        source << "\tconst size_t gidy = get_global_id(1);\n";
        source << "\tif (gidy >= " << shape[ndim-2] << ")\n\t\treturn;\n";
    }
    source << "\tconst size_t gidx = get_global_id(0);\n";
    source << "\tif (gidx >= " << shape[ndim-1] << ")\n\t\treturn;\n";
}

void generateOffsetSource(const cphvb_array* operand, std::ostream& source)
{
    cphvb_index ndim = operand->ndim;
    assert(ndim > 0);
    if (ndim > 2)
    {
        source << "gidz*" << operand->stride[ndim-3] << " + ";
    }
    if (ndim > 1)
    {
        source << "gidy*" << operand->stride[ndim-2] << " + ";
    }
    source << "gidx*" << operand->stride[ndim-1] << " + " << operand->start;
}

void generateInstructionSource(cphvb_opcode opcode,
                               OCLtype returnType,
                               std::vector<std::string>& parameters, 
                               std::ostream& source)
{
    assert(parameters.size() == (size_t)cphvb_operands(opcode));
    const char* type_str = oclTypeStr(returnType);
    switch(opcode)
    {
    case CPHVB_ADD:
        source << "\t" << parameters[0] << " = " << parameters[1] << " + " << parameters[2] << ";\n";
        break;
    case CPHVB_SUBTRACT:
        source << "\t" << parameters[0] << " = " << parameters[1] << " - " << parameters[2] << ";\n";
        break;
    case CPHVB_MULTIPLY:
        source << "\t" << parameters[0] << " = " << parameters[1] << " * " << parameters[2] << ";\n";
        break;
    case CPHVB_DIVIDE:
        source << "\t" << parameters[0] << " = " << parameters[1] << " / " << parameters[2] << ";\n";
        break;
    case CPHVB_LOGADDEXP:
        source << "\t" << parameters[0] << " = log(exp(" << parameters[1] << ") + exp(" 
               << parameters[2] << "));\n";
        break;
    case CPHVB_LOGADDEXP2:
        source << "\t" << parameters[0] << " = log2(exp2(" << parameters[1] << ") + exp2(" 
               << parameters[2] << "));\n";
        break;
    case CPHVB_TRUE_DIVIDE:
        source << "\t" << parameters[0] << " = (" << type_str << ")" << parameters[1] << " / (" 
               << type_str << ")" << parameters[2] << ";\n";
        break;
    case CPHVB_FLOOR_DIVIDE:
        source << "\t" << parameters[0] << " = floor(" << parameters[1] << " / " << parameters[2] << ");\n";
        break;
    case CPHVB_NEGATIVE:
        source << "\t" << parameters[0] << " = -" << parameters[1] << ";\n";
        break;
    case CPHVB_POWER:
        source << "\t" << parameters[0] << " = pow(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_REMAINDER:
        source << "\t" << parameters[0] << " = remainder(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_MOD:
        source << "\t" << parameters[0] << " = " << parameters[1] << " % " << parameters[2] << ";\n";
        break;
    case CPHVB_FMOD:
        source << "\t" << parameters[0] << " = fmod(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_ABSOLUTE:
        if (isFloat(returnType))
            source << "\t" << parameters[0] << " = fabs(" << parameters[1] << ");\n";
        else
            source << "\t" << parameters[0] << " = abs(" << parameters[1] << ");\n";
        break;
    case CPHVB_RINT:
        source << "\t" << parameters[0] << " = rint(" << parameters[1] << ");\n";
        break;
    case CPHVB_SIGN:
        source << "\t" << parameters[0] << " = " << parameters[1] << "<0?-1:1;\n";
        break;
    case CPHVB_EXP:
        source << "\t" << parameters[0] << " = exp(" << parameters[1] << ");\n";
        break;
    case CPHVB_EXP2:
        source << "\t" << parameters[0] << " = exp2(" << parameters[1] << ");\n";
        break;
    case CPHVB_LOG:
        source << "\t" << parameters[0] << " = log(" << parameters[1] << ");\n";
        break;
    case CPHVB_LOG10:
        source << "\t" << parameters[0] << " = log10(" << parameters[1] << ");\n";
        break;
    case CPHVB_EXPM1:
        source << "\t" << parameters[0] << " = expm1(" << parameters[1] << ");\n";
        break;
    case CPHVB_LOG1P:
        source << "\t" << parameters[0] << " = log1p(" << parameters[1] << ");\n";
        break;
    case CPHVB_SQRT:
        source << "\t" << parameters[0] << " = sqrt(" << parameters[1] << ");\n";
        break;
    case CPHVB_SQUARE:
        source << "\t" << parameters[0] << " = " << parameters[1] << " * " << parameters[1] << ";\n";
        break;
    case CPHVB_RECIPROCAL:
        source << "\t" << parameters[0] << " = 1/" << parameters[1] << ";\n";
        break;
    case CPHVB_SIN:
        source << "\t" << parameters[0] << " = sin(" << parameters[1] << ");\n";
        break;
    case CPHVB_COS:
        source << "\t" << parameters[0] << " = cos(" << parameters[1] << ");\n";
        break;
    case CPHVB_TAN:
        source << "\t" << parameters[0] << " = tan(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCSIN:
        source << "\t" << parameters[0] << " = asin(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCCOS:
        source << "\t" << parameters[0] << " = acos(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCTAN:
        source << "\t" << parameters[0] << " = atan(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCTAN2:
        source << "\t" << parameters[0] << " = atan2(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_HYPOT:
        source << "\t" << parameters[0] << " = hypot(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_SINH:
        source << "\t" << parameters[0] << " = sinh(" << parameters[1] << ");\n";
        break;
    case CPHVB_COSH:
        source << "\t" << parameters[0] << " = cosh(" << parameters[1] << ");\n";
        break;
    case CPHVB_TANH:
        source << "\t" << parameters[0] << " = tanh(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCSINH:
        source << "\t" << parameters[0] << " = asinh(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCCOSH:
        source << "\t" << parameters[0] << " = acosh(" << parameters[1] << ");\n";
        break;
    case CPHVB_ARCTANH:
        source << "\t" << parameters[0] << " = atanh(" << parameters[1] << ");\n";
        break;
    case CPHVB_DEG2RAD:
        source << "\t" << parameters[0] << " = " << parameters[1] << " * M_PI / 180.0;\n";
        break;
    case CPHVB_RAD2DEG:
        source << "\t" << parameters[0] << " = 180.0 * " << parameters[1] << " / M_PI;\n";
        break;
    case CPHVB_BITWISE_AND:
        source << "\t" << parameters[0] << " = " << parameters[1] << " & " << parameters[2] << ";\n";
        break;
    case CPHVB_BITWISE_OR:
        source << "\t" << parameters[0] << " = " << parameters[1] << " | " << parameters[2] << ";\n";
        break;
    case CPHVB_BITWISE_XOR:
        source << "\t" << parameters[0] << " = " << parameters[1] << " ^ " << parameters[2] << ";\n";
        break;
    case CPHVB_LOGICAL_NOT:
        source << "\t" << parameters[0] << " = !" << parameters[1] << ";\n";
        break;
    case CPHVB_LOGICAL_AND:
        source << "\t" << parameters[0] << " = " << parameters[1] << " && " << parameters[2] << ";\n";
        break;
    case CPHVB_LOGICAL_OR:
        source << "\t" << parameters[0] << " = " << parameters[1] << " || " << parameters[2] << ";\n";
        break;
    case CPHVB_LOGICAL_XOR:
        source << "\t" << parameters[0] << " = !" << parameters[1] << " != !" << parameters[2] << ";\n";
        break;
    case CPHVB_INVERT:
        source << "\t" << parameters[0] << " = ~" << parameters[1] << ";\n";
        break;
    case CPHVB_LEFT_SHIFT:
        source << "\t" << parameters[0] << " = " << parameters[1] << " << " << parameters[2] << ";\n";
        break;
    case CPHVB_RIGHT_SHIFT:
        source << "\t" << parameters[0] << " = " << parameters[1] << " >> " << parameters[2] << ";\n";
        break;
    case CPHVB_GREATER:
        source << "\t" << parameters[0] << " = " << parameters[1] << " > " << parameters[2] << ";\n";
        break;
    case CPHVB_GREATER_EQUAL:
        source << "\t" << parameters[0] << " = " << parameters[1] << " >= " << parameters[2] << ";\n";
        break;
    case CPHVB_LESS:
        source << "\t" << parameters[0] << " = " << parameters[1] << " < " << parameters[2] << ";\n";
        break;
    case CPHVB_LESS_EQUAL:
        source << "\t" << parameters[0] << " = " << parameters[1] << " <= " << parameters[2] << ";\n";
        break;
    case CPHVB_NOT_EQUAL:
        source << "\t" << parameters[0] << " = " << parameters[1] << " != " << parameters[2] << ";\n";
        break;
    case CPHVB_EQUAL:
        source << "\t" << parameters[0] << " = " << parameters[1] << " == " << parameters[2] << ";\n";
        break;
    case CPHVB_MAXIMUM:
        source << "\t" << parameters[0] << " = max(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_MINIMUM:
        source << "\t" << parameters[0] << " = min(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_IDENTITY:
        source << "\t" << parameters[0] << " = " << parameters[1] << ";\n";
        break;
    case CPHVB_SIGNBIT:
        source << "\t" << parameters[0] << " = " << parameters[1] << " < 0;\n";
        break;
    case CPHVB_LDEXP:
        source << "\t" << parameters[0] << " = ldexp(" << parameters[1] << ", " << parameters[2] << ");\n";
        break;
    case CPHVB_FLOOR:
        source << "\t" << parameters[0] << " = floor(" << parameters[1] << ");\n";
        break;
    case CPHVB_CEIL:
        source << "\t" << parameters[0] << " = ceil(" << parameters[1] << ");\n";
        break;
    case CPHVB_TRUNC:
        source << "\t" << parameters[0] << " = trunc(" << parameters[1] << ");\n";
        break;
    case CPHVB_LOG2:
        source << "\t" << parameters[0] << " = log2(" << parameters[1] << ");\n";
        break;
    default:
#ifdef DEBUG
        std::cerr << "Instruction \"" << cphvb_opcode_text(opcode) << "\" not supported." << std::endl;
#endif
        throw std::runtime_error("Instruction not supported.");
    }
}