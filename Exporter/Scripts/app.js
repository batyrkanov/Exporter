function execute() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var parts = []

    $("span.query-content").each(function () {
        parts.push($(this).text());
    });

    var query = parts.join(' ');

    var parameters = [];
    var name = "";
    var value = "";

    $(".parameter").each(function () {
        name = $(this).data("name");
        value = $(this).val();

        parameters.push(name + "-xyz-" + value);
    });

    $.ajax({
        url: "/Query/Execute",
        type: "POST",
        data: {
            __RequestVerificationToken: token,
            "input": query,
            "parameters": parameters
        },
        cache: false,
        success: function (result) {
            $("#result").html(result);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;
}

function userExec() {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var query = $("#SqlQueryContent").val();
    var parameters = [];

    $(".parameter").each(function () {
        name = $(this).data("name");
        value = $(this).val();

        parameters.push(name + "-xyz-" + value);
    });

    $.ajax({
        url: "/Query/Execute",
        type: "POST",
        data: {
            __RequestVerificationToken: token,
            "input": query,
            "parameters": parameters
        },
        cache: false,
        success: function (result) {
            $("#result").html(result);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;
}

function GetCsv() {
    var query = $("#SqlQueryContent").val();
    var parameters = [];

    $(".parameter").each(function () {
        name = $(this).data("name");
        value = $(this).val();

        parameters.push(name + "-xyz-" + value);
    });

    $.ajax({
        url: "/Query/FormCsvFile",
        type: "POST",
        data: {
            "input": query,
            "parameters": parameters
        },
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.fileName != "") {
                window.location.href = "/Query/GetFile?file=" + data.fileName + "&type=csv";
            } else if (data.errorMessage != null && data.errorMessage != "") {
                errorCase(null, "error", data.errorMessage);
            }
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;
}

function GetExcel() {
    var query = $("#SqlQueryContent").val();
    var parameters = [];

    $(".parameter").each(function () {
        name = $(this).data("name");
        value = $(this).val();

        parameters.push(name + "-xyz-" + value);
    });

    $.ajax({
        url: "/Query/FormExcelFile",
        type: "POST",
        data: {
            "input": query,
            "parameters": parameters
        },
        cache: false,
        dataType: "json",
        success: function (data) {
            if (data.fileName != "") {
                window.location.href = "/Query/GetFile?file=" + data.fileName + "&type=excel";
            } else if (data.errorMessage != null && data.errorMessage != "") {
                errorCase(null, "error", data.errorMessage);
            }
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest)
        }
    });
    return false;
}

function addParameter() {
    $.ajax({
        url: "/Parameter/CreateForm",
        type: "GET",
        cache: false,
        success: function (result) {
            $("#create-param").html(result);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest, "create-param");
        }
    });
    return false;
}

function createParam() {
    var name = $("#ParameterName").val();
    var ruName = $("#ParameterRuName").val();
    var type = $("#ParameterType").val();
    var token = $('input[name="__RequestVerificationToken"]').val();


    if ((name === null || name == "") || (ruName === null || ruName == "") || (type === null || type == "")) {

        alert("Заполните поля создания параметра");
        return false;
    }

    $.ajax({
        url: "/Parameter/CreateParameter",
        type: "POST",
        cache: false,
        data: {
            __RequestVerificationToken: token,
            "name": name,
            "ruName": ruName,
            "type": parseInt(type, 10)
        },
        success: function (parameterId) {
            generateInput(parameterId);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;
}

function generateInput(parameterId) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: "/Parameter/GenerateInput",
        type: "POST",
        cache: false,
        data: {
            __RequestVerificationToken: token,
            "parameterId": parseInt(parameterId, 10)
        },
        success: function (field) {
            $("#fields").append(field);
            $("#create-param").empty();
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest, "fields");
        }
    });
    return false;
}

function updateInput(parameterId, parentId) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: "/Parameter/GenerateInput",
        type: "POST",
        cache: false,
        data: {
            __RequestVerificationToken: token,
            "parameterId": parseInt(parameterId, 10)
        },
        success: function (field) {
            $("#" + parentId).html(field);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;
}

function removeParameter(parameterId) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: "/Parameter/RemoveParameter",
        type: "POST",
        cache: false,
        data: {
            __RequestVerificationToken: token,
            "parameterId": parseInt(parameterId, 10)
        },
        success: function (id) {
            $("#" + id).remove();
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;
}

function editParameter(btnId) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    var button = $("#" + btnId);
    var parameterId = button.data("id");
    var parentId = button.data("parentid");

    $.ajax({
        url: "/Parameter/EditParameter",
        type: "POST",
        cache: false,
        data: {
            __RequestVerificationToken: token,
            "parameterId": parseInt(parameterId, 10)
        },
        success: function (editForm) {
            $("#" + parentId).html(editForm);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest, parentId);
        }
    });
    return false;
}

function saveParamChanges(parentId) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    var parameterId = $("#ParameterId").val();
    var name = $("#ParameterName").val();
    var ruName = $("#ParameterRuName").val();
    var type = $("#ParameterType").val();

    if (name === null || ruName === null || type === null) {
        alert("Заполните поля создания параметра");
        return false;
    }

    $.ajax({
        url: "/Parameter/SaveChanges",
        type: "POST",
        cache: false,
        data: {
            __RequestVerificationToken: token,
            "parameterId": parseInt(parameterId, 10),
            "name": name,
            "ruName": ruName,
            "type": parseInt(type, 10)        
        },
        success: function (parameterId) {
            updateInput(parameterId, parentId);
        },
        error: function (XMLHttpRequest) {
            errorCase(XMLHttpRequest);
        }
    });
    return false;

}

function errorCase(info, id = "error", message = "Ошибка! Обновите страницу") {
    var message = '<div class="alert alert-danger">'+message+'</div >';
    $("#" + id).html(message);
    console.log(info);
}

function editorInit() {
    var code = $(".SqlQueryContent")[0];
    window.onload = function () {
        var mime = 'text/x-mariadb';
        // get mime type
        if (window.location.href.indexOf('mime=') > -1) {
            mime = window.location.href.substr(window.location.href.indexOf('mime=') + 5);
        }
        window.editor = CodeMirror.fromTextArea(code, {
            mode: mime,
            indentWithTabs: true,
            smartIndent: true,
            lineNumbers: true,
            matchBrackets: true,
            autofocus: true,
            styleActiveLine: true,
            theme: "dracula",
            extraKeys: { "Ctrl-Space": "autocomplete" },
            hintOptions: {
                tables: {
                    users: ["name", "score", "birthDate"],
                    countries: ["name", "population", "size"]
                }
            }

        });
    };
}

$(document).ready(function () {
    $('#create-form').on('submit', function (e) {
        var form = $(this);
        e.preventDefault();
        var elements = $(".validation-span");
        var validated = true;
        $.each(elements, function (e, v) {
            validated = $(v).is(':empty');

            if (!validated) {
                $("#se-pre-con").hide();
                return false;
            }
        });

        if (!validated) {
            return false;
        }

        $('#btn').blur();
        $('input').blur();
        $("#se-pre-con").show();
        form.off('submit');
        form.submit();
    });
    $("a[name='deleteButton']").click(function () {
        $("#se-pre-con").show();
        setTimeout(function () {
            $("#se-pre-con").hide();
            $('.delete-form').on('submit', function (e) {
                var form = $(this);
                e.preventDefault();
                $('#btn1').blur();
                $('input').blur();
                $("#se-pre-con").show();
                form.off('submit');
                form.submit();
            });
        }, 100)
    });
});

function disableBtn() {
    document.getElementById("Save").setAttribute("disabled", "disabled");
}
function myFunction(val, v2) {
    val = val.replace(/[\u200B-\u200D\uFEFF]/g, '');
    if (val.length == 0 || v2.length == 0) {
        document.getElementById("Save").setAttribute("disabled", "disabled");
    }
    else {
        document.getElementById("Save").removeAttribute("disabled", "disabled");
    }
}
function onPageLeave()
{
    $(document).ready(function () {
        var warn_on_unload = "";
        $('#add-button').on('click', function () {
            addEvent();
        });
        addEvent();
    });

    addEvent = function () {
        setTimeout(function () {
                

            $('input:text,input:checkbox,input:radio,textarea,select').off('change');
            $('input:text,input:checkbox,input:radio,textarea,select').on('change', function () {
                warn_on_unload = "Leaving this page will cause any unsaved data to be lost.";
                $('#Save').click(function (e) {
                    warn_on_unload = "";
                });
                myFunction($('span.query-content').text(), $('.show-btn').val());
                    
                window.onbeforeunload = function () {
                    if (warn_on_unload != '') {
                        return warn_on_unload;
                    }
                }
            });
                
            $('div.CodeMirror-code.query-content').off('DOMSubtreeModified');
            $('div.CodeMirror-code.query-content').on('DOMSubtreeModified', function () {
                warn_on_unload = "Leaving this page will cause any unsaved data to be lost.";
                $('#Save').click(function (e) {
                    warn_on_unload = "";
                });
                myFunction($('span.query-content').text(), $('.show-btn').val());

                window.onbeforeunload = function () {
                    if (warn_on_unload != '') {
                        return warn_on_unload;
                    }
                }
            })

        }, 100)
    }
}