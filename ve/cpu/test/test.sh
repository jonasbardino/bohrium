#!/usr/bin/env bash

WHERE=`pwd`
rm -r ~/.local/cpu/kernels
rm -r ~/.local/cpu/objects
INSTALLDIR="~/.local" DEBUG="" make clean install
cd $WHERE
#python ./test/3d_reduction.py]
#python ./test/crap.py
#python ~/Desktop/bohrium/benchmark/Python/shallow_water.py --size=3000*3000*2 --bohrium=False
#python ~/Desktop/bohrium/benchmark/Python/shallow_water.py --size=3000*3000*2 --bohrium=True
#python ~/Desktop/bohrium/benchmark/Python/shallow_water.py --size=3000*3000*2 --bohrium=True
#python ~/Desktop/bohrium/benchmark/Python/shallow_water.py --size=3000*3000*2 --bohrium=True
#BH_VE_CPU_DUMPSRC=1 python ../../test/numpy/numpytest.py
BH_VE_CPU_JIT_PRELOAD=0 BH_VE_CPU_JIT_ENABLED=1 BH_VE_CPU_JIT_FUSION=0 BH_VE_CPU_JIT_OPTIMIZE=0 BH_VE_CPU_JIT_DUMPSRC=1 python ../../test/numpy/numpytest.py -f test_reduce.py
../../bridge/cpp/bin/hello_world
#BH_VE_CPU_DUMPSRC=1 ../../bridge/cpp/bin/hello_world
#python ../../test/numpy/numpytest.py
#python ../../test/numpy/numpytest.py -f test_benchmarks.py
#python ../../test/numpy/numpytest.py -f test_matmul.py
#python ../../test/numpy/numpytest.py -f test_array_create.py
#python ../../test/numpy/numpytest.py -f test_primitives.py
#python ../../test/numpy/numpytest.py -f test_specials.py
#python ../../test/numpy/numpytest.py -f test_types.py
#python ../../test/numpy/numpytest.py -f test_views.py
