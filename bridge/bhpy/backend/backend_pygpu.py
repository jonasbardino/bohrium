"""
The Computation Backend

"""
import numpy as np
from .. import bhc
from .._util import dtype_name
import mmap
import time
import os
import pygpu
from pygpu.array import gpuarray as elemary
import backend_numpy

cxt = pygpu.init("opencl0:0")
#cxt = pygpu.init("cuda0")
pygpu.set_default_context(cxt)

class base(backend_numpy.base):
    """base array handle"""
    def __init__(self, size, dtype):
        self.clary = pygpu.empty((size,), dtype=dtype, cls=elemary)
        super(base, self).__init__(size, dtype)

class view(backend_numpy.view):
    """array view handle"""
    def __init__(self, ndim, start, shape, stride, base):
        super(view, self).__init__(ndim, start, shape, stride, base)
        self.clary = pygpu.gpuarray.from_gpudata(base.clary.gpudata, offset=self.start,\
                dtype=base.dtype, shape=shape, strides=self.stride, writable=True, base=base.clary, cls=elemary)

def views2clary(views):
    ret = []
    for v in views:
        if isinstance(v, view):
            ret.append(v.clary)
        else:
            ret.append(v)
    return ret

def get_data_pointer(ary, allocate=False, nullify=False):
    ary.ndarray[:] = np.asarray(ary.clary)
    return ary.ndarray.ctypes.data

def set_bhc_data_from_ary(self, ary):
    self.clary[:] = pygpu.asarray(ary)


ufunc_cmds = {'identity' : "i1",
              'add' : "i1+i2",
              'subtract' : "i1-i2",
              'multiply' : "i1*i2",
              'divide' : "i1/i2",
              'power' : "i1**i2",
              'absolute' : "abs(i1)",
              'sqrt' : "sqrt(i1)",
              }

def ufunc(op, *args):
    """Apply the 'op' on args, which is the output followed by one or two inputs"""
    args = views2clary(args)

    out=args[0]
    i1=args[1];
    if len(args) > 2:
        i2=args[2]

    if op.info['name'] == "identity":
        if out.base is i1.base:#PyGPU does not support inplace copy (it seems)
            i1 = i1.copy()
        out[:] = i1
    elif op.info['name'] in ufunc_cmds:
        cmd = "out[:] = %s"%ufunc_cmds[op.info['name']]
        exec cmd
    else:
        raise NotImplementedError()

def reduce(op, out, a, axis):
    """reduce 'axis' dimension of 'a' and write the result to out"""

    (a, out) = views2clary((a, out))
    if op.info['name'] == 'add':
        rfun = a.sum
    elif op.info['name'] == 'multiply':
        rfun = a.prod
    else:
        raise NotImplementedError()
    if a.ndim == 1:
        out[:] = rfun(axis=axis)
    else:
        rfun(axis=axis, out=out)

def accumulate(op, out, a, axis):
    """accumulate 'axis' dimension of 'a' and write the result to out"""

    raise NotImplementedError()

def extmethod(name, out, in1, in2):
    """Apply the extended method 'name' """

    raise NotImplementedError()

def range(size, dtype):
    """create a new array containing the values [0:size["""
    raise NotImplementedError()
    return np.arange((size,), dtype=dtype)

def random123(size, start_index, key):
    """Create a new random array using the random123 algorithm.
    The dtype is uint64 always."""
    raise NotImplementedError()
    return np.random.random(size)


import atexit
@atexit.register
def shutdown():
#    print "ufunc:", t_ufunc
#    print "vcache size at exit: %d"%len(vcache)
    pass