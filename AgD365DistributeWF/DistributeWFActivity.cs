using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;

/**
 * Copyright 2015 by Alberto Gemin
 * agemin@hotmail.com
 * Version 1.3.1.1
 * 7 Feb 2015
 **/
namespace Gemina.CRM2015.WF.CrmDistributeWF
{
    abstract public class DistributeWFActivity : CodeActivityBase
    {

        #region Workflow Parameters

        /// <summary>
        /// Input parameter: Workflow to be executed
        /// </summary>
        [Input("Distributed Workflow")]
        [ReferenceTarget("workflow")]
        [RequiredArgument]
        public InArgument<EntityReference> Workflow { get; set; }

        /// <summary>
        /// Input parameter: Name of the relationship
        /// </summary>
        [Input("Relationship Name")]
        [RequiredArgument]
        public InArgument<String> RelationshipName { get; set; }

        #endregion

        #region CodeActivity

        override protected bool ExecuteBody(CodeActivityContext executionContext)
        {
            this.Distribute(executionContext);

            return true;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Launch workflows for dependent entities.
        /// </summary>
        protected void Distribute(CodeActivityContext executionContext)
        {
            var keyList = this.GatherKeys(executionContext);
            var workflowId = this.Workflow.Get(executionContext).Id;

            var svc = this.GetService(executionContext);
            foreach (Guid key in keyList)
            {
                ExecuteWorkflowRequest workflowRequest = new ExecuteWorkflowRequest();
                workflowRequest.EntityId = key;
                workflowRequest.WorkflowId = workflowId;
                svc.Execute(workflowRequest);
            }
        }

        /// <summary>
        /// Get IDs of dependent entities
        /// </summary>
        protected abstract ICollection<Guid> GatherKeys(CodeActivityContext executionContext);

        #endregion

    }
}
