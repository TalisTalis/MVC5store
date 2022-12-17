$(function () {
    /* add new category */
    /* Объявляем и инициализируем нужные переменные */
    var newCatA = $("a#newcata");  /* Класс линка добавления */
    var newCatTextInput = $("#newcatname"); /* Класс текстового поля ввода */
    var ajaxText = $("span.ajax-text"); /* Класс картинки загрузки */
    var table = $("table#categories tbody") /* Класс таблицы ввода */

    /* Пишем функцию на отлов нажатия Enter */
    newCatTextInput.keyup(function (e) {
        if (e.keyCode == 13) {
            newCatA.click();
        }
    });

    /* Пишем функцию Click */
    newCatA.click(function (e) {
        e.preventDefault();

        var catName = newCatTextInput.val().trim();

        /* Проверка на длину названия категории, не меньше 3 символов */
        if (catName.length < 3) {
            alert("Category name must be at least 3 characters long.");
            return false;
        }

        /* Показать картинку загрузки */
        ajaxText.show();

        /* Путь до метода создания новой категории */
        var url = "/Admin/Shop/AddNewCategory/";

        /* отправка запроса на сервер */
        $.post(url, { catName: catName }, function (data) {
            var response = data.trim(); /* удаление пробелов впереди и сзади */
            /* Проверка на уникальность названия категории */
            if (response == "titletaken") {
                /* В блок вставить сообщение пользователю */
                ajaxText.html("<span class='alert alert-danger'>That title is taken!</span>");
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/images/ajax-loader.gif' height='50'/>");
                    });
                }, 2000);
                return false;
            }
            else {
                ajaxText.html("<span class='alert alert-success'>The category has been added");
                if (!$("table#categories").length) {
                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
                else {
                    setTimeout(function () {
                        ajaxText.fadeOut("fast", function () {
                            ajaxText.html("<img src='/Content/images/ajax-loader.gif' height='50'");
                        });
                    }, 2000);

                    newCatTextInput.val("");

                    var toAppend = $("table#categories tbody tr:last").clone();
                    toAppend.attr("id", "id_" + data);
                    toAppend.find("#item_Name").val(catName);
                    toAppend.find("a.delete").attr("href", "#");
                    toAppend.find("a.delete").attr("id", "delcata_" + data);
                    table.append(toAppend);
                    //table.sortable("refresh");
                    location.reload();
                }
            }
        });
    });
    //////////////////////////////////////////////////////////////////////////////////////////////

    /* confirm category deletion */
    $("body").on("click", "a.delete", function () {
        if (!confirm("Confirm category deletion")) return false;
        var ajaxText = $("span.ajax-text"); /* Класс картинки загрузки */
        var id = $(this).attr("id").substr(8);
        ajaxText.show();
        var url = "/Admin/Shop/DeleteCategory/";
        $.post(url, { id: id }, function (data) {
            if (!(id == data)) {
                return false;
            }
            else {
                ajaxText.html("<span class='alert alert-success'>The category has been deleted");
                $("#id_" + data).fadeOut();
                setTimeout(function () {
                    ajaxText.fadeOut("fast", function () {
                        ajaxText.html("<img src='/Content/images/ajax-loader.gif' height='50'");
                    });
                }, 2000);
                setTimeout(function () {
                    location.reload();
                }, 1000);
            }
        });
    });
    ////////////////////////////////////////////////////////////////////////////////////////////////

    /* change cursor when drag and drop */
    $("tr:not(.home)").mousedown(function () {
        $(this).css("cursor", "move");
    });
    $("tr:not(.home)").mouseup(function () {
        $(this).css("cursor", "default");
    });
    //////////////////////////////////////////////////////////////////////////////////////////////////

    /* Rename category script */
    $("table#categories input.text-box").dblclick(function () {
        //input_id = $(this).attr("class").replace(" form-control text-box single-line", "").substr(6);
        //alert(input_id.replace(" form-control text-box single-line","").substr(6));
        originalTextBoxValue = /*$(".input_" + input_id).val();*/$(this).val();
        //alert(originalTextBoxValue);
        $(this)./*$(".input_" + input_id).*/attr("readonly", false);
    });
    $("table#categories input.text-box").keyup(function (e) {
        //var input_id = $(this).attr("class").replace(" form-control text-box single-line", "").substr(6);
        if (e.keyCode == 13) {
            $this = $(this);
                    /*$(this).*/edit($this);
        }
    });
    function edit(this_input) {
        //var input_id = $(this).attr("class").replace(" form-control text-box single-line", "").substr(6);
        var $this = this_input;/* $(this);/*$(".input_" + input_id);*/
        var ajaxdiv = $this.parent().parent().parent().parent().find(".ajaxdivtd");
        var newCatName = $this.val().trim();
        var id = $this.parent().parent().parent().parent().parent().attr("id").substring(3);
        var url = "/Admin/Shop/RenameCategory";

        if (newCatName.length < 3) {
            alert("Category name must be at least 3 3character long.");
            $this.attr("readonly", true);
            return false;
        }

        $.post(url, { newCatName: newCatName, id: id }, function (data) {
            var respons = data.trim();
            if (respons == "titletaken") {
                $this.val(originalTextBoxValue);
                ajaxdiv.html("<div class='alert alert-danger'>That title is taken!</div>").show();
            }
            else {
                ajaxdiv.html("<div class='alert alert-success'>The category name been changed!</div>").show();
            }

            setTimeout(function () {
                ajaxdiv.fadeOut("fast", function () {
                    ajaxdiv.html("");
                });
            }, 3000);
        }).done(function () {
            $this.attr("readonly", true);
            setTimeout(function () {
                location.reload();
            }, 2000);
        });
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////

    /* Sorting script */
    $("table#categories tbody").sortable({
        items: "tr:not(.home)",
        placeholder: "ui-state-highlight",
        update: function () {
            var ids = $("table#categories tbody").sortable("serialize");
            var url = "/Admin/Shop/ReorderCategories";
            $.post(url, ids, function (data) {
            });
        }
    });
});