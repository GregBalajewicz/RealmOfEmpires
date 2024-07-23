<%@ Page Language="C#"  Inherits="BasePageWithRes" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Collections" %>

(function (obj, $, undefined) { 
    
    <%

        IEnumerable o2 = R.GetResourceSet( System.Globalization.CultureInfo.CurrentCulture, true, true).Cast<DictionaryEntry>();                    
         
         foreach(DictionaryEntry dt in o2) {

    %>
    obj['<%=dt.Key%>'] = '<%=dt.Value%>';
    <%}%>


} (window.BDA.Dict = window.BDA.Dict || {}, jQuery));
