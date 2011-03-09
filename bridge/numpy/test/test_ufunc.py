import numpy as np
import numpytest
import random

type=np.float32

def run():
    max_ndim = 6
    for i in xrange(1,max_ndim+1):
        src = numpytest.random_list(random.sample(xrange(1, 10),i))
        Ad = np.array(src, dtype=type, dist=True)
        Af = np.array(src, dtype=type, dist=False)
        ran = random.randint(0,i-1)
        if i > 1 and ran > 0:
            for j in range(0,ran):
                src = src[0]
        Bd = np.array(src, dtype=type, dist=True)
        Bf = np.array(src, dtype=type, dist=False)
        Cd = Ad + Bd + 42 + Bd[-1]
        Cf = Af + Bf + 42 + Bf[-1]
        Cd = Cd[::2] + Cd[::2,...] + Cd[0,np.newaxis]
        Cf = Cf[::2] + Cf[::2,...] + Cf[0,np.newaxis]
        Dd = np.array(Cd, dtype=type, dist=True)
        Df = np.array(Cf, dtype=type, dist=False)
        Dd[1:] = Cd[:-1]
        Df[1:] = Cf[:-1]
        Cd = Dd + Bd[np.newaxis,-1]
        Cf = Df + Bf[np.newaxis,-1]
        Cd[1:] = Cd[:-1]
        Cf[1:] = Cf[:-1]
        if not numpytest.array_equal(Cd,Cf):
            raise Exception("Uncorrect result array\n")

    for i in xrange(3,max_ndim+3):
        src = numpytest.random_list([i,i,i])
        Ad = np.array(src, dist=True, dtype=type)
        Af = np.array(src, dist=False, dtype=type)
        Bd = np.array(src, dist=True, dtype=type)
        Bf = np.array(src, dist=False, dtype=type)
        Cd = Ad[::2, ::2, ::2] + Bd[::2, ::2, ::2] + Ad[::2,1,2]
        Cf = Af[::2, ::2, ::2] + Bf[::2, ::2, ::2] + Af[::2,1,2]
        if not numpytest.array_equal(Cd,Cf):
            raise Exception("Uncorrect result array\n")

if __name__ == "__main__":
    run()
