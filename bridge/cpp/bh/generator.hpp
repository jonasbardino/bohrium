/*
This file is part of Bohrium and copyright (c) 2012 the Bohrium team:
http://bohrium.bitbucket.org

Bohrium is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as 
published by the Free Software Foundation, either version 3 
of the License, or (at your option) any later version.

Bohrium is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the 
GNU Lesser General Public License along with Bohrium. 

If not, see <http://www.gnu.org/licenses/>.
*/
#ifndef __BOHRIUM_BRIDGE_CPP_GENERATOR
#define __BOHRIUM_BRIDGE_CPP_GENERATOR

#include <initializer_list>

namespace bh {

int64_t unpack_shape(int64_t *shape, size_t index, size_t arg)
{
    shape[index] = arg;
    return 0;
}

template <typename ...Args>
int64_t unpack_shape(int64_t *shape, size_t index, size_t arg, Args... args)
{
    shape[index] = arg;
    unpack_shape(shape, ++index, args...);

    return 1;
}

template <typename T, typename ...Dimensions>
multi_array<T>& generator(const Dimensions&... shape)
{
    multi_array<T>* result = &Runtime::instance()->temp<T>();
    size_t count = sizeof...(Dimensions);
    int64_t shapes[16];

    std::cout << "GOT= " << count << std::endl;
    unpack_shape(shapes, 0, shape...);
    for(size_t i=0; i<count; i++) {
        std::cout << "GOT= " << shapes[i] << std::endl;
    }

    return *result;
}

template <typename T, typename ...Dimensions>
multi_array<T>& empty(const Dimensions&... shape)
{
    int64_t ndim = sizeof...(Dimensions);
    multi_array<T>* result = &Runtime::instance()->temp<T>();

    return *result;
}

template <typename T>
multi_array<T>& ones()
{
    multi_array<T>* result = &Runtime::instance()->temp<T>();

    result = (T)1;

    return *result;
}

template <typename T>
multi_array<T>& zeros()
{
    multi_array<T>* result = &Runtime::instance()->temp<T>();

    result = (T)0;

    return *result;
}

template <typename T>
multi_array<T>& range(size_t start, size_t end, size_t skip)
{
    multi_array<T>* result = &Runtime::instance()->temp<T>();
    std::cout << "range(" << start << "," << end << "," << skip << ");" << std::endl;

    return *result;
}

template <typename T>
multi_array<T>& random(int n)
{
    bh_random_type* rinstr = (bh_random_type*)malloc(sizeof(bh_random_type));
    if (rinstr == NULL) {
        char err_msg[100];
        sprintf(err_msg, "Failed alllocating memory for extension-call.");
        throw std::runtime_error(err_msg);
    }

    multi_array<T>* result = &Runtime::instance()->temp<T>((size_t)n);

    rinstr->id          = Runtime::instance()->random_id;        //Set the instruction
    rinstr->nout        = 1;
    rinstr->nin         = 0;
    rinstr->struct_size = sizeof(bh_random_type);
    rinstr->operand[0]  = &storage[result->getKey()];

    Runtime::instance()->enqueue<T>((bh_userfunc*)rinstr);

    return *result;
}

}
#endif

