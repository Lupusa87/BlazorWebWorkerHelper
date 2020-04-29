var WebWorkers_array = [];
var tmpValue1;


function WwOnError(e, wwID, dotnethelper) {
    dotnethelper.invokeMethodAsync('InvokeOnError', e.message);
}


function WwOnMessage(e, wwID, dotnethelper) {
 
    if (e.data.isBinary) {

        tmpValue1 = e.data.binarydata;

        e.data.binarydata = null;

        Module.mono_call_static_method('[BlazorWebWorkerHelper] BlazorWebWorkerHelper.StaticClass:AllocateArray',
            [tmpValue1.byteLength, wwID, JSON.stringify(e.data)]);
    }
    else {

        dotnethelper.invokeMethodAsync('InvokeOnMessage', JSON.stringify(e.data));
       
    }
}



window.BwwJsFunctions = {
  alert: function (message) {
      return alert(message);
  },
  showPrompt: function (message) {
    return prompt(message, 'Type anything here');
  },
  WwAddDedicated: function (obj) {

        var b = {
            id: obj.wwID,
            name: '',
            ww: new Worker(obj.wwUrl),
            type:0
        };

        obj.dotnethelper.invokeMethodAsync('InvokeStateChanged', 1);


      b.ww.onmessage = function (e) { WwOnMessage(e, obj.wwID, obj.dotnethelper); };

      b.ww.addEventListener('error', function (e) { WwOnError(e, obj.wwID, obj.dotnethelper); }, false);
       
        WebWorkers_array.push(b);


        return true;
    },
    WwAddShared: function (obj) {

        var b = {
            id: obj.wwID,
            name: obj.wwName,
            ww: new SharedWorker(obj.wwUrl, obj.wwName),
            type:1
        };

        obj.dotnethelper.invokeMethodAsync('InvokeStateChanged', 1);


        b.ww.port.onmessage = function (e) { WwOnMessage(e, obj.wwID, obj.dotnethelper); };

        b.ww.addEventListener('error', function (e) { WwOnError(e, obj.wwID, obj.dotnethelper); }, false);

        WebWorkers_array.push(b);

        return true;
    },
    WwRemove: function (WwID) {
        var result = true;
        var index = WebWorkers_array.findIndex(x => x.id === WwID);

        if (index > -1) {
            WebWorkers_array[index].ww.terminate();
            WebWorkers_array.splice(index, 1);
        }
        else {
            result = false;
        }

        return result;
    },
    WwSendDedicated: function (obj) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === obj.wwID);

        if (index > -1) {

            WebWorkers_array[index].ww.postMessage({ cmd: obj.wCommandType, msg: obj.wwMessage, args: obj.additionalArgs });
            result = true;

        }

        return result;
    },
    WwSendDedicatedBinary: function (id, bag, data) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === BINDING.conv_string(id));

        if (index > -1) {

            b = JSON.parse(BINDING.conv_string(bag));
          
            //it is cloning arraybuffer, direct without cloning was giving error!
            buffer = new Uint8Array(Array.from(Blazor.platform.toUint8Array(data))).buffer;
           
            WebWorkers_array[index].ww.postMessage({ cmd: b.cmd, msg: buffer, args: b.args }, [buffer]);

            result = true;

        }

        return result;
    },
    WwSendShared: function (obj) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === obj.wwID);

        if (index > -1) {

            WebWorkers_array[index].ww.port.postMessage({ cmd: obj.wCommandType, msg: obj.wwMessage, args: obj.additionalArgs});
            result = true;
        }

        return result;
    },
    WwSendSharedBinary: function (id, bag, data) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === BINDING.conv_string(id));

        if (index > -1) {
           
            b = JSON.parse(BINDING.conv_string(bag));

            //it is cloning arraybuffer, direct without cloning was giving error!
            buffer = new Uint8Array(Array.from(Blazor.platform.toUint8Array(data))).buffer;

            WebWorkers_array[index].ww.port.postMessage({ cmd: b.cmd, msg: buffer, args: b.args }, [buffer]);
            
            result = true;

        }

        return result;
    },
    GetBinaryData: function (d) {

        var destinationUint8Array = Blazor.platform.toUint8Array(d);
        destinationUint8Array.set(new Uint8Array(tmpValue1));

        tmpValue1 = null;
        return true;
    },
};
