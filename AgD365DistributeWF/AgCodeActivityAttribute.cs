using System;

/**
 * Copyright 2015 by Alberto Gemin
 * agemin@hotmail.com
 * Version 1.3.1.1
 * 7 Feb 2015
 **/
namespace Gemina.CRM2015.WF
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AgCodeActivityAttribute : Attribute
    {

        #region Fields

        private string _groupName;
        private string _name;

        #endregion

        #region Properties

        public string GroupName
        {
            get
            {
                return this._groupName;
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
        }

        #endregion

        #region Constructors

        public AgCodeActivityAttribute(string name, string groupName)
        {
            this._name = name;
            this._groupName = groupName;
        }

        #endregion

    }
}

