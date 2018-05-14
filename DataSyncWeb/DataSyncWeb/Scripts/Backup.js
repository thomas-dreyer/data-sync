var number = 0;
var inc = 0;
var id = 0;
var current = 0;

$.ready(function () {
    $("#btnBackup").click(function () {
        startBackup();
    });
   
});

function getProgress() {
    $.ajax({
        url: "Home/Progress",
        type: "POST",
        data: { Id: id },
        success: function (result) {
            clearInterval(inc);
            writeLog(result.Progress)
            console.log(result.Progress);
            if (result.Complete === true) {
                clearInterval(number);
            }
        },
        error: function (result) {
            console.log(result);
        }
    });
}

function incrementLog(target, element, increment) {
    if (current >= target) {
        clearInterval(inc);
        current = target;
    }
    else {
        current +=increment;
    }
    element.innerHTML = current.toFixed(2) + " %";
}

function writeLog(log) {
    logDiv = document.getElementById("logDiv");
    rate = (log - current) / 10;
    inc = setInterval(incrementLog, 55, log, logDiv, rate);
}

function startBackup() {
    $("#btnBackup").enabled = false;
    var _source = document.getElementById("source").value;
    var _destination = document.getElementById("destination").value;
    $.ajax({
        url: "Home/Backup",
        type: "POST",
        data: {
            source: _source,
            destination: _destination
        },
        success: function (result) {
            if (result.Id > 0) {
                id = result.Id;
                current = 0;
                number = setInterval(getProgress, 1000);
            }
        }
    });
}

    

