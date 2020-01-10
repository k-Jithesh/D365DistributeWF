using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

/**
 * Copyright 2015 by Alberto Gemin
 * agemin@hotmail.com
 * Version 1.3.1.1
 * 7 Feb 2015
 **/
namespace Gemina.CRM2015.WF
{
    [Serializable]
    public class AgErrorMessageException : Exception
    {

        #region Constructors

        public AgErrorMessageException(string text) : base(text) { }
        public AgErrorMessageException(string text, Exception ex) : base(text, ex) { }
        public AgErrorMessageException() : base() { }
        protected AgErrorMessageException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }

        #endregion

    }
}
