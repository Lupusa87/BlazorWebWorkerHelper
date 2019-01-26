var WebWorkers_array = [];



function WwOnError(e, wwID, dotnethelper) {
    dotnethelper.invokeMethodAsync('InvokeOnError', e.message);
}


function WwOnMessage(e, wwID, dotnethelper) {
   
    if (e.data.isBinary) {

        var a1 = new TextEncoder("utf-8").encode(JSON.stringify(e.data));

        var allocateArrayMethod = Blazor.platform.findMethod(
            'BlazorWebWorkerHelper',
            'BlazorWebWorkerHelper',
            'StaticClass',
            'AllocateArray'
        );

        var dotNetArray = Blazor.platform.callMethod(allocateArrayMethod,
            null,
            [Blazor.platform.toDotNetString(a1.byteLength.toString())]);

        var arr = Blazor.platform.toUint8Array(dotNetArray);

        arr.set(new Uint8Array(a1));

        var receiveResponseMethod = Blazor.platform.findMethod(
            'BlazorWebWorkerHelper',
            'BlazorWebWorkerHelper',
            'StaticClass',
            'HandleMessageBinary'
        );

        Blazor.platform.callMethod(receiveResponseMethod,
            null,
            [dotNetArray, Blazor.platform.toDotNetString(wwID)]);

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
            ww: new SharedWorker(obj.wwUrl),
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

            WebWorkers_array[index].ww.postMessage({ cmd: obj.wCommandType, msg: obj.wwMessage });
            result = true;

        }

        return result;
    },
    WwSendDedicatedBinary: function (id, cmd, data) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === Blazor.platform.toJavaScriptString(id));

        if (index > -1) {

            arr = Blazor.platform.toUint8Array(data);

            WebWorkers_array[index].ww.postMessage({ cmd: cmd, msg: arr });
            result = true;

        }

        return result;
    },
    WwSendShared: function (obj) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === obj.wwID);

        if (index > -1) {

            WebWorkers_array[index].ww.port.postMessage({ cmd:obj.wCommandType, msg:obj.wwMessage });
            result = true;
        }

        return result;
    },
    WwSendSharedBinary: function (id, cmd, data) {
        var result = false;

        var index = WebWorkers_array.findIndex(x => x.id === Blazor.platform.toJavaScriptString(id));

        if (index > -1) {

            arr = Blazor.platform.toUint8Array(data);
               
            WebWorkers_array[index].ww.port.postMessage({ cmd: cmd, msg: arr });
            result = true;

        }

        return result;
    },
};
