 //put into Assets/Plugins/JSlib.jslib - https://answers.unity.com/questions/1095407/saving-webgl.html
 var JSLib = {
	 
	   SendExternalTransaction: function (to,value, gas, gasPrice, data) {
		  if (typeof web3 !== 'undefined') {
			  console.log("send transac" + Pointer_stringify(gas));
			web3js = new Web3(web3.currentProvider);
			web3js.eth.sendTransaction({
				from: web3js.eth.accounts[0],
				to: Pointer_stringify(to),
				value: Pointer_stringify(value),
				gas: Pointer_stringify(gas),
				gasPrice: Pointer_stringify(gasPrice),
				data: Pointer_stringify(data)
			}, function(error,result){console.log(result)});
		  } else {
			console.log("no provider");
		  }
  },
  
	 /////////IO Handler
     SyncFiles : function()
     {
		window.alert("Launch");
         FS.syncfs(false,function (err) {
			window.alert("Bonjour");
         });
     },
	 
	 ////////Provider function
	 	GetProvider: function(){
		if (typeof web3 !== 'undefined') {
			return true;
		}
		else{
			return false;
		}
	},
		GetExternalAddress: function() {
		  if (typeof web3 !== 'undefined') {
			web3js = new Web3(web3.currentProvider);
			var returnStr = web3js.eth.accounts[0];
			var bufferSize = lengthBytesUTF8(returnStr) + 1;
			var buffer = _malloc(bufferSize);
			stringToUTF8(returnStr, buffer, bufferSize);
			return buffer;
		  } else {
			console.log("no provider");
			return "";
		  }
    }
	/*SendTransaction: function(to,value) {
		window.alert(Pointer_stringify(to));
		window.alert(Pointer_stringify(value));
		  if (typeof web3 !== 'undefined') {
			web3js = new Web3(web3.currentProvider);
			web3js.eth.sendTransaction({
				from: web3js.eth.accounts[0],
				to: Pointer_stringify(to),
				value: Pointer_stringify(value)
			}, function(error,result){console.log(result)});
		  } else {
			console.log("no provider");
		  }
    }*/
 };
 
mergeInto(LibraryManager.library, JSLib);
