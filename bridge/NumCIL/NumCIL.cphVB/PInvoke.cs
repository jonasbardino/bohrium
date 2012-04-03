﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//CPHVB basic control types
using cphvb_intp = System.Int64;
using cphvb_index = System.Int64;
using cphvb_type = System.Int64;
using cphvb_enumbase = System.Int32;
using cphvb_data_ptr = System.IntPtr;

//CPHVB Signed data types
using cphvb_bool = System.SByte;
using cphvb_int8 = System.SByte;
using cphvb_int16 = System.Int16;
using cphvb_int32 = System.Int32;
using cphvb_int64 = System.Int64;

//CPHVB Unsigned data types
using cphvb_uint8 = System.Byte;
using cphvb_uint16 = System.UInt16;
using cphvb_uint32 = System.UInt32;
using cphvb_uint64 = System.UInt64;
using cphvb_float32 = System.Single;
using cphvb_float64 = System.Double;

namespace NumCIL.cphVB
{
    public static class PInvoke
    {
        public const int CPHVB_COM_NAME_SIZE = 1024;
        public const int CPHVB_MAXDIM = 16;
        public const int CPHVB_MAX_EXTRA_META_DATA = 1024;
        public const int CPHVB_MAX_NO_INST = 100;
        public const int CPHVB_MAX_NO_OPERANDS = 3;

        public static readonly bool Is64Bit = IntPtr.Size == 8;
        public static readonly int INTP_SIZE = Marshal.SizeOf(typeof(cphvb_intp));

        public enum cphvb_com_type : long
        {
            CPHVB_BRIDGE,
            CPHVB_VEM,
            CPHVB_VE,
            CPHVB_COMPONENT_ERROR
        }

        public enum cphvb_error : long
        {
            CPHVB_SUCCESS,
            CPHVB_ERROR,
            CPHVB_TYPE_ERROR,
            CPHVB_TYPE_NOT_SUPPORTED,
            CPHVB_TYPE_NOT_SUPPORTED_BY_OP,
            CPHVB_TYPE_COMB_NOT_SUPPORTED,
            CPHVB_OUT_OF_MEMORY,
            CPHVB_RESULT_IS_CONSTANT,
            CPHVB_OPERAND_UNKNOWN,
            CPHVB_ALREADY_INITALIZED,
            CPHVB_NOT_INITALIZED,
            CPHVB_PARTIAL_SUCCESS,
            CPHVB_INST_DONE,
            CPHVB_INST_UNDONE,
            CPHVB_INST_NOT_SUPPORTED,
            CPHVB_INST_NOT_SUPPORTED_FOR_SLICE,
        }

        public enum cphvb_opcode : long
        {
            CPHVB_ADD,
            CPHVB_SUBTRACT,
            CPHVB_MULTIPLY,
            CPHVB_DIVIDE,
            CPHVB_LOGADDEXP,
            CPHVB_LOGADDEXP2,
            CPHVB_TRUE_DIVIDE,
            CPHVB_FLOOR_DIVIDE,
            CPHVB_NEGATIVE,
            CPHVB_POWER,
            CPHVB_REMAINDER,
            CPHVB_MOD,
            CPHVB_FMOD,
            CPHVB_ABSOLUTE,
            CPHVB_RINT,
            CPHVB_SIGN,
            CPHVB_CONJ,
            CPHVB_EXP,
            CPHVB_EXP2,
            CPHVB_LOG,
            CPHVB_LOG10,
            CPHVB_EXPM1,
            CPHVB_LOG1P,
            CPHVB_SQRT,
            CPHVB_SQUARE,
            CPHVB_RECIPROCAL,
            CPHVB_ONES_LIKE,
            CPHVB_SIN,
            CPHVB_COS,
            CPHVB_TAN,
            CPHVB_ARCSIN,
            CPHVB_ARCCOS,
            CPHVB_ARCTAN,
            CPHVB_ARCTAN2,
            CPHVB_HYPOT,
            CPHVB_SINH,
            CPHVB_COSH,
            CPHVB_TANH,
            CPHVB_ARCSINH,
            CPHVB_ARCCOSH,
            CPHVB_ARCTANH,
            CPHVB_DEG2RAD,
            CPHVB_RAD2DEG,
            CPHVB_BITWISE_AND,
            CPHVB_BITWISE_OR,
            CPHVB_BITWISE_XOR,
            CPHVB_LOGICAL_NOT,
            CPHVB_LOGICAL_AND,
            CPHVB_LOGICAL_OR,
            CPHVB_LOGICAL_XOR,
            CPHVB_INVERT,
            CPHVB_LEFT_SHIFT,
            CPHVB_RIGHT_SHIFT,
            CPHVB_GREATER,
            CPHVB_GREATER_EQUAL,
            CPHVB_LESS,
            CPHVB_LESS_EQUAL,
            CPHVB_NOT_EQUAL,
            CPHVB_EQUAL,
            CPHVB_MAXIMUM,
            CPHVB_MINIMUM,
            CPHVB_ISFINITE,
            CPHVB_ISINF,
            CPHVB_ISNAN,
            CPHVB_SIGNBIT,
            CPHVB_MODF,
            CPHVB_LDEXP,
            CPHVB_FREXP,
            CPHVB_FLOOR,
            CPHVB_CEIL,
            CPHVB_TRUNC,
            CPHVB_LOG2,
            CPHVB_ISREAL,
            CPHVB_ISCOMPLEX,
            CPHVB_IDENTITY,
            CPHVB_USERFUNC,//It is an user-defined function
            CPHVB_RELEASE, // ==     CPHVB_SYNC + CPHVB_DISCARD
            CPHVB_SYNC,    //Inform child to make data synchronized and available.
            CPHVB_DISCARD, //Inform child to forget the array
            CPHVB_DESTROY, //Inform child to deallocate the array.
            CPHVB_RANDOM,  //file out with random
            CPHVB_ARANGE, // out, start, step
            //Used by a brigde to mark untranslatable operations.
            //NB: CPHVB_NONE must be the last element in this enum.
            CPHVB_NONE
        }

        public enum cphvb_type : long
        {
            CPHVB_BOOL,
            CPHVB_INT8,
            CPHVB_INT16,
            CPHVB_INT32,
            CPHVB_INT64,
            CPHVB_UINT8,
            CPHVB_UINT16,
            CPHVB_UINT32,
            CPHVB_UINT64,
            CPHVB_FLOAT16,
            CPHVB_FLOAT32,
            CPHVB_FLOAT64,
            CPHVB_INDEX, // Not a data type same as INT32 used for e.g. reduce dim
            //NB: CPHVB_UNKNOWN must be the last element in this enum.
            CPHVB_UNKNOWN
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
        struct dictionary
        {
            public int        n ;     /** Number of entries in dictionary */
            public int        size ;  /** Storage size */
            public byte[][]  val ;   /** List of string values */
            public byte[][]    key ;   /** List of string keys */
            public uint[]   hash ;  /** List of hash values for keys */
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct cphvb_constant
        {
            [FieldOffset(0)] public cphvb_bool     bool8;
            [FieldOffset(0)] public cphvb_int8     int8;
            [FieldOffset(0)] public cphvb_int16    int16;
            [FieldOffset(0)] public cphvb_int32    int32;
            [FieldOffset(0)] public cphvb_int64    int64;
            [FieldOffset(0)] public cphvb_uint8    uint8;
            [FieldOffset(0)] public cphvb_uint16   uint16;
            [FieldOffset(0)] public cphvb_uint32   uint32;
            [FieldOffset(0)] public cphvb_uint64   uint64;
            [FieldOffset(0)] public cphvb_float32  float32;
            [FieldOffset(0)] public cphvb_float64  float64;

            public cphvb_constant Set(cphvb_bool v) { this.bool8 = v; return this; }
            //public cphvb_constant Set(cphvb_int8 v) { this.int8 = v; return this; }
            public cphvb_constant Set(cphvb_int16 v) { this.int16 = v; return this; }
            public cphvb_constant Set(cphvb_int32 v) { this.int32 = v; return this; }
            public cphvb_constant Set(cphvb_int64 v) { this.int64 = v; return this; }
            public cphvb_constant Set(cphvb_uint8 v) { this.uint8 = v; return this; }
            public cphvb_constant Set(cphvb_uint16 v) { this.uint16 = v; return this; }
            public cphvb_constant Set(cphvb_uint32 v) { this.uint32 = v; return this; }
            public cphvb_constant Set(cphvb_uint64 v) { this.uint64 = v; return this; }
            public cphvb_constant Set(cphvb_float32 v) { this.float32 = v; return this; }
            public cphvb_constant Set(cphvb_float64 v) { this.float64 = v; return this; }
            public cphvb_constant Set(object v) 
            {
                if (v is cphvb_bool)
                    return Set((cphvb_bool)v);
                else if (v is cphvb_int16)
                    return Set((cphvb_int16)v);
                else if (v is cphvb_int32)
                    return Set((cphvb_int32)v);
                else if (v is cphvb_int64)
                    return Set((cphvb_int64)v);
                else if (v is cphvb_uint8)
                    return Set((cphvb_uint8)v);
                else if (v is cphvb_uint16)
                    return Set((cphvb_uint16)v);
                else if (v is cphvb_uint32)
                    return Set((cphvb_uint32)v);
                else if (v is cphvb_uint64)
                    return Set((cphvb_uint64)v);
                else if (v is cphvb_float32)
                    return Set((cphvb_float32)v);
                else if (v is cphvb_float64)
                    return Set((cphvb_float64)v);

                throw new NotSupportedException(); 
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct cphvb_data_array
        {
            [FieldOffset(0)] private cphvb_bool[]     bool8;
            [FieldOffset(0)] private cphvb_int8[]     int8;
            [FieldOffset(0)] private cphvb_int16[]    int16;
            [FieldOffset(0)] private cphvb_int32[]    int32;
            [FieldOffset(0)] private cphvb_int64[]    int64;
            [FieldOffset(0)] private cphvb_uint8[]    uint8;
            [FieldOffset(0)] private cphvb_uint16[]   uint16;
            [FieldOffset(0)] private cphvb_uint32[]   uint32;
            [FieldOffset(0)] private cphvb_uint64[]   uint64;
            [FieldOffset(0)] private cphvb_float32[]  float32;
            [FieldOffset(0)] private cphvb_float64[]  float64;
            [FieldOffset(0)] private IntPtr           voidPtr;

            public cphvb_data_array Set(cphvb_bool[] v) { this.bool8 = v; return this; }
            //public cphvb_data_array Set(cphvb_int8[] v) { this.int8 = v; return this; }
            public cphvb_data_array Set(cphvb_int16[] v) { this.int16 = v; return this; }
            public cphvb_data_array Set(cphvb_int32[] v) { this.int32 = v; return this; }
            public cphvb_data_array Set(cphvb_int64[] v) { this.int64 = v; return this; }
            public cphvb_data_array Set(cphvb_uint8[] v) { this.uint8 = v; return this; }
            public cphvb_data_array Set(cphvb_uint16[] v) { this.uint16 = v; return this; }
            public cphvb_data_array Set(cphvb_uint32[] v) { this.uint32 = v; return this; }
            public cphvb_data_array Set(cphvb_uint64[] v) { this.uint64 = v; return this; }
            public cphvb_data_array Set(cphvb_float32[] v) { this.float32 = v; return this; }
            public cphvb_data_array Set(cphvb_float64[] v) { this.float64 = v; return this; }
            public cphvb_data_array Set(IntPtr v) { this.voidPtr = v; return this; }
            public cphvb_data_array Set(object v) { throw new NotSupportedException(); }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
        public struct cphvb_com
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=CPHVB_COM_NAME_SIZE)]
            public byte[] name;
            public IntPtr config;  /*dictionary *config;*/
            public IntPtr lib_handle; //Handle for the dynamic linked library.
            public cphvb_com_type type;
            public cphvb_init init;
            public cphvb_shutdown shutdown;
            public cphvb_execute execute;
            public cphvb_reg_func reg_func;
            public cphvb_create_array create_array; //Only for VEMs

#if DEBUG
            /// <summary>
            /// Converts the Asciiz name to a string, used for debugging only
            /// </summary>
            public string Name { get { return System.Text.Encoding.ASCII.GetString(this.name.TakeWhile(b => !b.Equals(0)).ToArray()); } }
#endif
        }

        /// <summary>
        /// Fake wrapper struct to keep a pointer to cphvb_array typesafe
        /// </summary>
        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Pack = 0)]
        public struct cphvb_array_ptr
        {
            /// <summary>
            /// The actual IntPtr value
            /// </summary>
            [FieldOffset(0)]
            private IntPtr m_ptr;

            /// <summary>
            /// Accessor methods to read/write the data pointer
            /// </summary>
            public IntPtr Data 
            {
                get 
                {
                    if (m_ptr == IntPtr.Zero)
                        throw new ArgumentNullException();

                    //IntPtr test = Marshal.ReadIntPtr(m_ptr, (Marshal.SizeOf(cphvb_intp) * (4 + (CPHVB_MAXDIM * 2))));

                    IntPtr res;
                    cphvb_error e = cphvb_data_get(this, out res);
                    if (e != cphvb_error.CPHVB_SUCCESS)
                        throw new cphVBException(e);
                    return res;
                }
                set
                {
                    if (m_ptr == IntPtr.Zero)
                        throw new ArgumentNullException();

                    cphvb_error e = cphvb_data_set(this, value);
                    if (e != cphvb_error.CPHVB_SUCCESS)
                        throw new cphVBException(e);
                }
            }

            /// <summary>
            /// Accessor methods to read/write the base array
            /// </summary>
            public cphvb_array_ptr BaseArray
            {
                get
                {
                    if (m_ptr == IntPtr.Zero)
                        throw new ArgumentNullException();
                    
                    return new cphvb_array_ptr() { 
                        m_ptr = Marshal.ReadIntPtr(m_ptr, INTP_SIZE)
                    };
                }
            }

            /// <summary>
            /// A value that represents a null pointer
            /// </summary>
            public static readonly cphvb_array_ptr Null = new cphvb_array_ptr() { m_ptr = IntPtr.Zero };

            /// <summary>
            /// Free's the array view, but does not de-reference it with the VEM
            /// </summary>
            public void Free()
            {
                if (m_ptr == IntPtr.Zero)
                    return;

                cphvb_com_free_ptr(m_ptr);
                m_ptr = IntPtr.Zero;
            }

            /// <summary>
            /// Custom equals functionality
            /// </summary>
            /// <param name="obj">The object to compare to</param>
            /// <returns>True if the objects are equal, false otherwise</returns>
            public override bool Equals(object obj)
            {
                if (obj is cphvb_array_ptr)
                    return ((cphvb_array_ptr)obj).m_ptr == this.m_ptr;
                else
                    return base.Equals(obj);
            }

            /// <summary>
            /// Custom hashcode functionality
            /// </summary>
            /// <returns>The hash code for this instance</returns>
            public override cphvb_int32 GetHashCode()
            {
                return m_ptr.GetHashCode();
            }

            /// <summary>
            /// Simple compare operator for pointer type
            /// </summary>
            /// <param name="a">One argument</param>
            /// <param name="b">Another argument</param>
            /// <returns>True if the arguments are the same, false otherwise</returns>
            public static bool operator ==(cphvb_array_ptr a, cphvb_array_ptr b)
            {
                return a.m_ptr == b.m_ptr;
            }

            /// <summary>
            /// Simple compare operator for pointer type
            /// </summary>
            /// <param name="a">One argument</param>
            /// <param name="b">Another argument</param>
            /// <returns>False if the arguments are the same, true otherwise</returns>
            public static bool operator !=(cphvb_array_ptr a, cphvb_array_ptr b)
            {
                return a.m_ptr != b.m_ptr;
            }

            public override string ToString()
            {
                return string.Format("(self: {0}, data: {1}, base: {2})", m_ptr, m_ptr == IntPtr.Zero ? "null" : this.Data.ToString(), m_ptr == IntPtr.Zero ? "null" : this.BaseArray.ToString());
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
        public struct cphvb_array
        {
            public cphvb_intp owner;
            public cphvb_array[] basearray;
            public cphvb_type type;
            public cphvb_intp ndim;
            public cphvb_index start;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=CPHVB_MAXDIM)]
            public cphvb_index[] shape;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=CPHVB_MAXDIM)]
            public cphvb_index[] stride;
            public cphvb_data_array data;
            public cphvb_intp has_init_value;
            public cphvb_constant init_value;
            public cphvb_intp ref_count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=CPHVB_MAX_EXTRA_META_DATA)]
            public byte[] extra_meta_data;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
        public struct cphvb_userfunc
        {
            public cphvb_intp id;
            public cphvb_intp nout;
            public cphvb_intp nin;
            public cphvb_intp struct_size;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=1)]
            public cphvb_array[] operand;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 0)]
        public struct cphvb_instruction
        {
            //Instruction status
            public cphvb_error status;
            //Opcode: Identifies the operation
            public cphvb_opcode opcode;
            //Id of each operand
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=CPHVB_MAX_NO_OPERANDS)]
            public cphvb_array_ptr[] operand;
            //Points to the user-defined function when the opcode is
            //CPHVB_USERFUNC.
            public IntPtr userfunc;

            public cphvb_instruction(cphvb_opcode opcode, cphvb_array_ptr operand)
            {
                this.status = cphvb_error.CPHVB_INST_UNDONE;
                this.opcode = opcode;
                this.userfunc = IntPtr.Zero;
                this.operand = new cphvb_array_ptr[CPHVB_MAX_NO_OPERANDS];
                this.operand[0] = operand;
            }

            public cphvb_instruction(cphvb_opcode opcode, cphvb_array_ptr operand1, cphvb_array_ptr operand2)
            {
                this.status = cphvb_error.CPHVB_INST_UNDONE;
                this.opcode = opcode;
                this.userfunc = IntPtr.Zero;
                this.operand = new cphvb_array_ptr[CPHVB_MAX_NO_OPERANDS];
                this.operand[0] = operand1;
                this.operand[1] = operand2;
            }

            public cphvb_instruction(cphvb_opcode opcode, cphvb_array_ptr operand1, cphvb_array_ptr operand2, cphvb_array_ptr operand3)
            {
                this.status = cphvb_error.CPHVB_INST_UNDONE;
                this.opcode = opcode;
                this.userfunc = IntPtr.Zero;
                this.operand = new cphvb_array_ptr[CPHVB_MAX_NO_OPERANDS];
                this.operand[0] = operand1;
                this.operand[1] = operand2;
                this.operand[2] = operand3;
            }

            public cphvb_instruction(cphvb_opcode opcode, IEnumerable<cphvb_array_ptr> operands)
            {
                this.status = cphvb_error.CPHVB_INST_UNDONE;
                this.opcode = opcode;
                this.userfunc = IntPtr.Zero;
                this.operand = new cphvb_array_ptr[CPHVB_MAX_NO_OPERANDS];
                int i = 0;
                foreach(var o in operands)
                    this.operand[i++] = o;
            }

            public override string ToString()
            {
                return string.Format("{0}({1}, {2}, {3})", this.opcode, operand[0], operand[1], operand[2]);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate cphvb_error cphvb_init(ref cphvb_com self);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate cphvb_error cphvb_shutdown();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate cphvb_error cphvb_execute(cphvb_intp count, cphvb_instruction[] inst_list);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate cphvb_error cphvb_reg_func(string lib, string fun, ref cphvb_intp id);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate cphvb_error cphvb_create_array(
                                   cphvb_array_ptr basearray,
                                   cphvb_type     type,
                                   cphvb_intp     ndim,
                                   cphvb_index    start,
                                   cphvb_index[]    shape,
                                   cphvb_index[]    stride,
                                   cphvb_intp     has_init_value,
                                   cphvb_constant init_value,
                                   out cphvb_array_ptr new_array);

        /// <summary>
        /// Setup the root component, which normally is the bridge.
        /// </summary>
        /// <returns>A new component object</returns>
        [DllImport("libcphvb", EntryPoint = "cphvb_com_setup", CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Auto)]
        private extern static IntPtr cphvb_com_setup_masked();

        /// <summary>
        /// Setup the root component, which normally is the bridge.
        /// </summary>
        /// <returns>A new component object</returns>
        public static cphvb_com cphvb_com_setup()
        {
            IntPtr p = cphvb_com_setup_masked();
            cphvb_com r = (cphvb_com)Marshal.PtrToStructure(p, typeof(cphvb_com));
            cphvb_com_free_ptr(p);
			return r;
        }

        /// <summary>
        /// Retrieves the children components of the parent.
        /// NB: the array and all the children should be free'd by the caller
        /// </summary>
        /// <param name="parent">The parent component (input)</param>
        /// <param name="count">Number of children components</param>
        /// <param name="children">Array of children components (output)</param>
        /// <returns>Error code (CPHVB_SUCCESS)</returns>
        [DllImport("libcphvb", EntryPoint = "cphvb_com_children", CallingConvention = CallingConvention.Cdecl, SetLastError = true, CharSet = CharSet.Auto)]
        private extern static cphvb_error cphvb_com_children_masked([In] ref cphvb_com parent, [Out] out cphvb_intp count, [Out] out IntPtr children);

        /// <summary>
        /// Retrieves the children components of the parent.
        /// NB: the array and all the children should be free'd by the caller
        /// </summary>
        /// <param name="parent">The parent component (input)</param>
        /// <param name="count">Number of children components</param>
        /// <param name="children">Array of children components (output)</param>
        /// <returns>Error code (CPHVB_SUCCESS)</returns>
        public static cphvb_error cphvb_com_children(cphvb_com parent, out cphvb_com[] children)
        {

            //TODO: Errors in setup may cause memory leaks, but we should terminate anyway

            IntPtr ch;
            long count = 0;
            children = null;

            cphvb_error e = cphvb_com_children_masked(ref parent, out count, out ch);
            if (e != cphvb_error.CPHVB_SUCCESS)
                return e;

            children = new cphvb_com[count];
            for (int i = 0; i < count; i++)
            {
                IntPtr cur = Marshal.ReadIntPtr(ch, Marshal.SizeOf(typeof(cphvb_intp)) * i);
                children[i] = (cphvb_com)Marshal.PtrToStructure(cur, typeof(cphvb_com));
                e = cphvb_com_free_ptr(cur);
                if (e != cphvb_error.CPHVB_SUCCESS)
                    return e;
            }

            e = cphvb_com_free_ptr(ch);
            if (e != cphvb_error.CPHVB_SUCCESS)
                return e;

            return e;
        }


        /// <summary>
        /// Frees the component
        /// </summary>
        /// <param name="component">The component to free</param>
        /// <returns>Error code (CPHVB_SUCCESS)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_com_free([In] ref cphvb_com component);

        /// <summary>
        /// Frees the component
        /// </summary>
        /// <param name="component">The component to free</param>
        /// <returns>Error code (CPHVB_SUCCESS)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_com_free_ptr([In] IntPtr component);
        
        /// <summary>
        /// Retrieves an user-defined function
        /// </summary>
        /// <param name="self">The component</param>
        /// <param name="lib">Name of the shared library e.g. libmyfunc.so, When NULL the default library is used.</param>
        /// <param name="func">Name of the function e.g. myfunc</param>
        /// <param name="ret_func">Pointer to the function (output), Is NULL if the function doesn't exist</param>
        /// <returns>Error codes (CPHVB_SUCCESS)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_com_get_func([In] ref cphvb_com self, [In] string lib, [In] string func,
                               [Out] out cphvb_userfunc ret_func);

        /// <summary>
        /// Trace an array creation
        /// </summary>
        /// <param name="self">The component</param>
        /// <param name="ary">The array to trace</param>
        /// <returns>Error code (CPHVB_SUCCESS)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_com_trace_array([In] ref cphvb_com self, [In] ref cphvb_array ary);


        /// <summary>
        /// Trace an instruction
        /// </summary>
        /// <param name="self">The component</param>
        /// <param name="inst">The instruction to trace</param>
        /// <returns>Error code (CPHVB_SUCCESS)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_com_trace_inst([In] ref cphvb_com self, [In] ref cphvb_instruction inst);

        /// <summary>
        /// Set the data pointer for the array.
        /// Can only set to non-NULL if the data ptr is already NULL
        /// </summary>
        /// <param name="array">The array in question</param>
        /// <param name="data">The new data pointer</param>
        /// <returns>Error code (CPHVB_SUCCESS, CPHVB_ERROR)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_data_set([In] cphvb_array_ptr array, [In] IntPtr data);

        /// <summary>
        /// Get the data pointer for the array.
        /// </summary>
        /// <param name="array">The array in question</param>
        /// <param name="data">The data pointer</param>
        /// <returns>Error code (CPHVB_SUCCESS, CPHVB_ERROR)</returns>
        [DllImport("libcphvb", SetLastError = true, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        public extern static cphvb_error cphvb_data_get([In] cphvb_array_ptr array, [Out] out IntPtr data);

    }
}