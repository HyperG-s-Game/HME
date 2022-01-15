using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WolfGamer.Utils{
    public class DebugCommandBase{
        private string commandId;
        private string commanDiscription;
        private string commandFormat;
        public string CommandId{get{return commandId;}}
        public string CommandDiscription{get{return commanDiscription;}}
        public string CommandFormat{get{return commandFormat;}}
        public DebugCommandBase(string _id,string _discription,string _format){
            this.commandId = _id;
            this.commanDiscription = _discription;
            this.commandFormat = _format;
        }
    }
    public class DebugCommand<TDebugCommand> : DebugCommandBase{
        private Action<TDebugCommand> command;
        public DebugCommand(string _id,string _discription,string _format,Action<TDebugCommand> _command) : base (_id,_discription,_format){
            this.command = _command;
        }
        public void InvokeCommandAction(TDebugCommand value){
            command.Invoke(value);
        }
        

    }
    public class DebugCommand : DebugCommandBase{
        private Action command;
        public DebugCommand(string _id,string _discription,string _format,Action _command) : base (_id,_discription,_format){
            this.command = _command;
        }
        public void InvokeCommandAction(){
            command.Invoke();
        }
        

    }

}