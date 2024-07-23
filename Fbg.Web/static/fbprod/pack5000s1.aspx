<%@ Page Language="C#" AutoEventWireup="true"  %>
<!DOCTYPE html>
<html>
 <head prefix=
    "og: http://ogp.me/ns# 
     fb: http://ogp.me/ns/fb# 
     product: http://ogp.me/ns/product#">
    <meta property="og:type"                   content="og:product" />
    <meta property="og:title"                  content="4500 Servants + 500 FREE (SALE)" />
    <meta property="og:plural_title"           content="4500 Servants + 500 FREE (SALE)" />
    <meta property="og:image"                  content="https://static.realmofempires.com/images/misc/fbcreditslogo.png" />
    <meta property="og:description"            content="Servants are used to unlock Premium Features" />
    <meta property="og:url"                    content="https://www.realmofempires.com/static/fbprod/pack4500s1.aspx" />
    <meta property="product:price:amount"      content="99.99"/>
    <meta property="product:price:currency"    content="USD"/>
     <%if (Config.SaleType != 1) Server.Transfer("pack1000.html"); %>
  </head>
</html>