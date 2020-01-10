using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;

/**
 * Copyright 2015 by Alberto Gemin
 * agemin@hotmail.com
 * Version 1.3.1.1
 * 7 Feb 2015
 **/
namespace Gemina.CRM2015.WF
{

    /// <summary>
    /// Custom base class for all Code Activities.
    /// </summary>
    public abstract class CodeActivityBase : CodeActivity
    {

        #region Properties

        /// <summary>
        /// Primary Key of the entity the WF runs on.
        /// </summary>
        public Guid CurrentRecordId { get; set; }

        /// <summary>
        /// Name of the entity the WF runs on.
        /// </summary>
        public string CurrentEntityName { get; set; }

        /// <summary>
        /// Operation that triggered the workflow.
        /// </summary>
        public string CurrentOperation { get; set; }

        #endregion

        #region CodeActivity

        /// <summary>
        /// Workflow step.
        /// </summary>
        protected override void Execute(CodeActivityContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            try
            {
                var workflowContext = context.GetExtension<IWorkflowContext>();
                this.CurrentRecordId = workflowContext.PrimaryEntityId;
                this.CurrentOperation = workflowContext.MessageName;
                this.CurrentEntityName = workflowContext.PrimaryEntityName;

                this.ExecuteBody(context);
            }
            catch (AgErrorMessageException ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, ex.Message);
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed, ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// Overridden in derived classes: contains the actual workflow step.
        /// </summary>
        protected abstract bool ExecuteBody(CodeActivityContext executionContext);

        #endregion

        #region Protected Methods

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        protected IOrganizationService GetService(ActivityContext executionContext, Guid? user)
        {
            if (executionContext == null)
            {
                throw new ArgumentNullException("executionContext");
            }
            var svcFactory = executionContext.GetExtension<IOrganizationServiceFactory>();

            if (user == null)
            {
                var workflowContext = executionContext.GetExtension<IWorkflowContext>();
                return svcFactory.CreateOrganizationService(workflowContext.UserId);
            }
            else
            {
                return svcFactory.CreateOrganizationService(user);
            }
        }

        protected IOrganizationService GetService(CodeActivityContext executionContext)
        {
            return this.GetService(executionContext, null);
        }

        #endregion

    }

}
