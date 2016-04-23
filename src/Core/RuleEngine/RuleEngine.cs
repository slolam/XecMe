using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities.Rules;
using XecMe.Common;

namespace XecMe.Core.RuleEngine
{
    public class RuleEngine
    {
        private IRulesStore _ruleStore;

        public RuleEngine(IRulesStore ruleStore)
        {
            Guard.ArgumentNotNull(ruleStore, "ruleStore");
            _ruleStore = ruleStore;
        }

        public void Execute<T>(RuleName ruleName, PolicyExecutionContext<T> context)
        {
            Guard.ArgumentNotNull(ruleName, "ruleName");
            Execute<T>(_ruleStore.GetRuleSet(ruleName), context);
        }

        public void Execute<T>(RuleSet ruleSet, PolicyExecutionContext<T> context)
        {
            Guard.ArgumentNotNull(ruleSet, "ruleSet");
            RuleValidation validation = new RuleValidation(context.GetType(),null);
            RuleExecution execution = new RuleExecution(validation, context);
            ruleSet.Execute(execution);
        }
    }
}
