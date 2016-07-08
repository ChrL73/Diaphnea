function getCookieExpires(days)
{
   var date = new Date();
   date.setTime(date.getTime() + days * 86400000);
   return "; expires=" + date.toGMTString();
}
