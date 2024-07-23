using Gmbc.Common.Diagnostics.ExceptionManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class FbPayment : ISerializableToNameValueCollection
{
    public long id;
    public FbPaymentItem[] items;
    public FbPaymentAction[] actions;
    public FbPaymentDispute[] disputes;
    public FbPaymentUser user;
    public FbPaymentRefundableAmount refundable_amount;

    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "id", id);
        BaseApplicationException.AddAdditionalInformation(col, "items", items);
        BaseApplicationException.AddAdditionalInformation(col, "actions", actions);
        BaseApplicationException.AddAdditionalInformation(col, "user", user);
    }

    #endregion
}

public class FbPaymentDispute : ISerializableToNameValueCollection
{
    public string user_comment;
    public string time_created;
    public string user_email;
    public string status;
    public string reason;

    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "user_comment", user_comment);
        BaseApplicationException.AddAdditionalInformation(col, "time_created", time_created);
        BaseApplicationException.AddAdditionalInformation(col, "user_email", user_email);
        BaseApplicationException.AddAdditionalInformation(col, "status", status);
        BaseApplicationException.AddAdditionalInformation(col, "reason", reason);
    }

    #endregion
}

public class FbPaymentRefundableAmount : ISerializableToNameValueCollection
{
    public string currency;
    public string amount;

    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "currency", currency);
        BaseApplicationException.AddAdditionalInformation(col, "amount", amount);
    }

    #endregion
}

public class FbPaymentAction : ISerializableToNameValueCollection
{
    public string type;
    public string status;
    public string currency;
    public string amount;

    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "type", type);
        BaseApplicationException.AddAdditionalInformation(col, "status", status);
        BaseApplicationException.AddAdditionalInformation(col, "currency", currency);
        BaseApplicationException.AddAdditionalInformation(col, "amount", amount);
    }

    #endregion
}

public class FbPaymentItem : ISerializableToNameValueCollection
{
    public string product;

    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "product", product);
    }

    #endregion
}

public class FbPaymentUser : ISerializableToNameValueCollection
{
    public string id;

    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "id", id);
    }

    #endregion
}

public class FbRealTimeRequest
{

    public FbRealTimeRequestEntry[] entry;
}

public class FbRealTimeRequestEntry : ISerializableToNameValueCollection
{
    public long id;
    public string[] changed_fields;
    #region ISerializableToNameValueCollection Members

    public void SerializeToNameValueCollection(System.Collections.Specialized.NameValueCollection col)
    {
        BaseApplicationException.AddAdditionalInformation(col, "id", id);
    }

    #endregion

}