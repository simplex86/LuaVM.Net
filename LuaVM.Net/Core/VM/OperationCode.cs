using System;
using System.Collections.Generic;

namespace LuaVM.Net.Core
{
    using Invoke = Action<Instruction, LuaState>;

    // 操作码
    struct OperationCode
    {
        internal byte testFlag;
        internal byte setAFlag;
        internal byte argBMode;
        internal byte argCMode;
        internal byte opMode;
        internal string name;
        internal Invoke invoke;

        public OperationCode(byte testFlag, 
                             byte aFlag, 
                             byte argBMode, 
                             byte argCMode, 
                             byte opMode, 
                             string name,
                             Invoke invoke)
        {
            this.testFlag = testFlag;
            this.setAFlag = aFlag;
            this.argBMode = argBMode;
            this.argCMode = argCMode;
            this.opMode = opMode;
            this.name = name;
            this.invoke = invoke;
        }
    }
}
