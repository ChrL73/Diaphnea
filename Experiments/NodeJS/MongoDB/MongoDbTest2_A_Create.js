var mongoose = require("mongoose");
var aSync = require("async");

var db = mongoose.connect("mongodb://localhost/testDb");

var n1 = 100;

var testSchema;
var TestModel;
var testArray;
var iArray;

fillArray();

function fillArray()
{
   console.log("n=" + n1 + " fillArray");
   
   testSchema = mongoose.Schema({ indexField: Number, stringField: String }, { collection: "test" + n1.toString() });
   TestModel = mongoose.model("Test" + n1.toString(), testSchema);
   testArray = [];
   iArray = [];
   
   var x = 0.5, r = 3.99;
   var i, j;
   for (i = 0; i < n1; ++i)
   {
      var s = "";
      for (j = 0; j < 8; ++j)
      {
         x = r * x * (1 - x);
         s += String.fromCharCode(26 * x + 97);
      }
      
      iArray.push(i);
      testArray.push(s + i.toString());
   }
   
   //console.log(testArray);
   //console.log(iArray);
   
   TestModel.remove({}, addDocuments);
}

function addDocuments(err)
{
   console.log("n=" + n1 + " addDocuments");
   
   if (err)
   {
      console.log(err);
   }
   else
   {
      aSync.each(iArray, function(j, callback)
      {
         TestModel.create({ "indexField": j, "stringField": testArray[j] }, function()
         {
            callback(false);
         });
      },
      function(err)
      {
         if (err)
         {
            console.log("err = " + err);
         }
         else
         {
            if (n1 < 30000)
            {
               n1 *= 2;
               fillArray();
            }
            else
            {
               console.log("End");
               process.exit();
            }
         }
      });
   }
}
