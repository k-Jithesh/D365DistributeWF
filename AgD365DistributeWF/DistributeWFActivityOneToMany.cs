using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

/**
 * Copyright 2015 by Alberto Gemin
 * agemin@hotmail.com
 * Version 1.3.1.1
 * 7 Feb 2015
 **/
namespace Gemina.CRM2015.WF.CrmDistributeWF
{
    [AgCodeActivity("Distribute One to Many", "DWF AG Utilities")]
    public sealed class DistributeWFActivityOneToMany : DistributeWFActivity
    {

        #region Protected Methods

        override protected ICollection<Guid> GatherKeys(CodeActivityContext executionContext)
        {
            var relationship = this.GetRelationship(executionContext);

            var query = new QueryByAttribute()
            {
                EntityName = relationship.ReferencingEntity
            };
            query.Attributes.Add(relationship.ReferencingAttribute);
            query.Values.Add(this.CurrentRecordId.ToString());

            var retrieveRequest = new RetrieveMultipleRequest()
            {
                Query = query
            };

            // Foreach object just get the primary key
            var keyList = new List<Guid>();
            var retrieveResponse = (RetrieveMultipleResponse)this.GetService(executionContext).Execute(retrieveRequest);
            foreach (var entity in retrieveResponse.EntityCollection.Entities)
            {
                keyList.Add(entity.Id);
            }

            return keyList;
        }

        #endregion

        #region Methods

        private OneToManyRelationshipMetadata GetRelationship(CodeActivityContext executionContext)
        {
            var relationshipRequest = new RetrieveRelationshipRequest()
            {
                Name = this.RelationshipName.Get(executionContext),
                RetrieveAsIfPublished = false
            };

            var relationshipResponse = (RetrieveRelationshipResponse)this.GetService(executionContext).Execute(relationshipRequest);
            if (!(relationshipResponse.RelationshipMetadata is OneToManyRelationshipMetadata))
            {
                throw new AgErrorMessageException("Relationship is not One to Many");
            }

            return (OneToManyRelationshipMetadata)relationshipResponse.RelationshipMetadata;
        }

        #endregion

    }
}
