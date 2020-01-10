using System;
using System.Collections.Generic;
using System.Linq;
using System.Activities;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

/**
 * Copyright 2015 by Alberto Gemin
 * agemin@hotmail.com
 * Version 1.3.1.1
 * 7 Feb 2015
 **/
namespace Gemina.CRM2015.WF.CrmDistributeWF
{
    [AgCodeActivity("Distribute Many to Many", "DWF AG Utilities")]
    public sealed class DistributeWFActivityManyToMany : DistributeWFActivity
    {

        #region Protected Methods

        protected override ICollection<Guid> GatherKeys(CodeActivityContext executionContext)
        {
            var relationship = this.GetRelationship(executionContext);

            string intersection = relationship.IntersectEntityName;
            if (relationship.Entity1LogicalName == this.CurrentEntityName && relationship.Entity2LogicalName == this.CurrentEntityName)
            {
                // N:N on the same entity
                var list1 = this.GatherKeysInternal(executionContext,
                    relationship.Entity1IntersectAttribute,
                    relationship.Entity2LogicalName,
                    relationship.Entity2IntersectAttribute,
                    intersection);
                var list2 = this.GatherKeysInternal(executionContext,
                    relationship.Entity2IntersectAttribute,
                    relationship.Entity1LogicalName,
                    relationship.Entity1IntersectAttribute,
                    intersection);

                var list = new HashSet<Guid>();
                foreach (var key in list1)
                {
                    if (object.Equals(key, this.CurrentRecordId))
                    {
                        // exclude this own entity
                    }
                    else
                    {
                        list.Add(key);
                    }
                }
                foreach (var key in list2)
                {
                    if (object.Equals(key, this.CurrentRecordId))
                    {
                        // exclude this own entity
                    }
                    else if (list.Contains(key))
                    {
                        // already there (?)
                    }
                    else
                    {
                        list.Add(key);
                    }
                }
                return list.ToList<Guid>();
            }
            else if (relationship.Entity1LogicalName == this.CurrentEntityName)
            {
                // entity1 is primary
                return this.GatherKeysInternal(executionContext,
                    relationship.Entity1IntersectAttribute,
                    relationship.Entity2LogicalName,
                    relationship.Entity2IntersectAttribute,
                    intersection);
            }
            else
            {
                // entity2 is primary
                return this.GatherKeysInternal(executionContext,
                    relationship.Entity2IntersectAttribute,
                    relationship.Entity1LogicalName,
                    relationship.Entity1IntersectAttribute,
                    intersection);
            }
        }

        #endregion

        #region Methods

        private ICollection<Guid> GatherKeysInternal(CodeActivityContext executionContext, string primaryAttribute, string secondaryEntity, string secondaryAttribute, string intersection)
        {
            var query = new QueryExpression();
            var secondaryToIntersection = new LinkEntity();
            var intersectionToPrimary = new LinkEntity();
            var primaryCondition = new ConditionExpression();

            // Chain all links
            query.EntityName = secondaryEntity;
            query.LinkEntities.Add(secondaryToIntersection);
            secondaryToIntersection.LinkEntities.Add(intersectionToPrimary);
            intersectionToPrimary.LinkCriteria.Conditions.Add(primaryCondition);

            // First link
            //secondaryToIntersection.LinkToEntityName = intersection;
            //secondaryToIntersection.LinkFromAttributeName =
            //secondaryToIntersection.LinkToAttributeName = secondaryAttribute;

            // First link -- Modified code
            secondaryToIntersection.LinkToEntityName = intersection;
            string relationName = RelationshipName.Get(executionContext);
            if (relationName == "listcontact_association")
            {
                secondaryToIntersection.LinkFromAttributeName = "contactid";
                secondaryToIntersection.LinkToAttributeName = secondaryAttribute;
            }
            else
            {
                secondaryToIntersection.LinkFromAttributeName =
                secondaryToIntersection.LinkToAttributeName = secondaryAttribute;
            }

            // Second link
            intersectionToPrimary.LinkToEntityName = this.CurrentEntityName;
            intersectionToPrimary.LinkFromAttributeName =
            intersectionToPrimary.LinkToAttributeName = primaryAttribute;

            // Condition
            primaryCondition.AttributeName = primaryAttribute;
            primaryCondition.Operator = ConditionOperator.Equal;
            primaryCondition.Values.Add(this.CurrentRecordId.ToString());

            var retrieveRequest = new RetrieveMultipleRequest()
            {
                Query = query
            };

            var list = new List<Guid>();
            var retrieveResponse = (RetrieveMultipleResponse)this.GetService(executionContext).Execute(retrieveRequest);
            foreach (var entity in retrieveResponse.EntityCollection.Entities)
            {
                list.Add(entity.Id);
            }

            return list;
        }

        private ManyToManyRelationshipMetadata GetRelationship(CodeActivityContext executionContext)
        {
            var relationshipRequest = new RetrieveRelationshipRequest()
            {
                Name = this.RelationshipName.Get(executionContext),
                RetrieveAsIfPublished = false
            };

            var relationshipResponse = (RetrieveRelationshipResponse)this.GetService(executionContext).Execute(relationshipRequest);
            if (!(relationshipResponse.RelationshipMetadata is ManyToManyRelationshipMetadata))
            {
                throw new AgErrorMessageException("Relationship is not Many to Many");
            }
            return (ManyToManyRelationshipMetadata)relationshipResponse.RelationshipMetadata;
        }

        #endregion

    }
}
