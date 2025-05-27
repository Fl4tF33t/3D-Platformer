using System;

namespace Platformer {
    public class FuncPredicate : IPredicate {
        private readonly Func<bool> func;
        public FuncPredicate(Func<bool> func) => this.func = func;
        public bool Evaluate() => func.Invoke();
    }
    
    // public class ActionPredicate : IPredicate {
    //     private readonly Action action;
    //     public ActionPredicate(Action action) => this.action = action;
    //     public bool Evaluate() {
    //         action.Invoke();
    //         return true;
    //     }
    // }
}