// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function warning(getID1, getID2, getID3, getID4, action, message) {
    Swal.fire({
        title: "Are you sure?",
        text: message,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#e03724',
        //cancelButtonColor: '#DD6B55',
        confirmButtonText: "Yes",
        cancelButtonText: "No"
    }).then((result) => {
        if (result['isConfirmed']) {
            console.log('Confirm');
            Delete(getID1, getID2, getID3, getID4, action)
            //Delete(getID1, action);
            //$.ajax({
            //    type: "POST",
            //    url: '@Url.Action("SaveData", "Home", new { mode = "" })',
            //    data: "11",
            //    success: function (config) {
            //        alert(config.c3);

            //        $("#loaderDiv").hide();
            //        $("#myModal1").modal("hide");
            //        swal.fire({
            //            title: 'SUCCESS',
            //            icon: 'success',
            //            text: "Send Mail Aleady",

            //        })
            //            .then((result) => {
            //                if (result.isConfirmed) {
            //                    var a = "?test=" + config.c1;
            //                    // $.get('https://api.ipify.org?format=json')
            //                    window.location.href = '@Url.Action("Index", "Home")' + a;
            //                }

            //            })



            //    }
            //});



        } else {
            console.log('Cancel');
            return false;
        }
    });
}

function warningSearch(getID, action, returnAction, message) {
    Swal.fire({
        title: "Are you sure?",
        text: message,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes",
        cancelButtonText: "No"
    }).then((result) => {
        if (result['isConfirmed']) {
            console.log('Confirm');
            console.log(getID);
            if (action == "DeleteFile") {
                fakeDelete(getID);
            }
            else {
                deleteSearch(getID, action, returnAction);
            }

        } else {
            console.log('Cancel');
            return false;
        }
    });
}

function edit(getID, action) {
    $.ajax({
        type: 'post',
        url: action,
        data: { id: getID },
        success: function (data) {
            $.each(data, function (i, item) {
                $('#modal-form').remove();
                var htmls = " <form id='modal-form'> "
                    + "<div class='form-group'>"
                    + "<input type='text' name='maNo' id='updateNo' value='" + item.maNo + "' readonly='readonly' style='display: none;' />"
                    + "</div>"
                    + "<div class='form-group' style='display:flex;'>"
                    + "<label for='recipient-name' class='col-form-label' style='padding: 10px;'>Area:</label>"
                    + "<input class='form-control' type='text' name='maArea' id='changeDetail' value='" + item.maArea + "'>"
                    + "</div>"
                    + "</form>"
                $('#modal-body').append(htmls);
            });
            $('#editModal').modal('show');
        }
    });
}

function update(getID, action) {
    $.ajax({
        type: 'post',
        url: action,
        data: $('#modal-form').serialize(),
        success: function (data) {
            console.log('message:' + data.res);
            if (data.res != '') {
                Swal.fire({
                    icon: data.res,
                    title: data.res,
                    text: data.res == "success" ? "Update Complete" : "Update not Complete",

                    //title: "Are you sure?",
                    //text: message,
                    //icon: "warning",
                    showConfirmButton: data.res == "success" ? true : false,
                    showCancelButton: data.res == "success" ? false : true,
                    confirmButtonText: "OK",
                    cancelButtonText: "NO"
                }).then((result) => {
                    if (result['isConfirmed']) {
                        console.log('Confirm');
                        window.location.href = data.path;
                        //Delete(getID, action);
                    } else {
                        console.log('Cancel');
                        return false;
                    }
                });
            }

            if (message != '') {
                if (message == 'success') {
                    $("#tr_" + $(getID).val()).attr('style', 'color:blue;');
                }
                Swal.fire({

                });
            }
        }
    });
}


//{
//    $.ajax({
//        type: 'post',
//        url: action,
//        data: $('#modal-form').serialize() ,
//        success: function (message) {
//            console.log('message:' + message);
//            if (message != '') {
//                    $("#tr_" + $(getID).val()).attr('style','color:blue;');
//                }
//                Swal.fire({
//                    icon: message,
//                    title: message,
//                    text: message == "success" ? "Update" : "Update not complete"
//                });
//        }
//    });
//}

function Delete(getID1, getID2, getID3, getID4, action) {
    $.ajax({
        type: 'post',
        url: action,
        data: { docno: getID1, stepno: getID2 },
        success: function (data) {
            console.log('message:' + data.res);
            if (data.res != '') {
                Swal.fire({
                    icon: data.res,
                    title: data.res,
                    text: data.res == "success" ? "Delete Complete" : "Delete not Complete",

                    //title: "Are you sure?",
                    //text: message,
                    //icon: "warning",
                    showConfirmButton: data.res == "success" ? true : false,
                    showCancelButton: data.res == "success" ? false : true,
                    confirmButtonText: "OK",
                    cancelButtonText: "NO"
                }).then((result) => {
                    if (result['isConfirmed']) {
                        console.log('Confirm');
                        window.location.href = data.path;
                        // Delete(getID1, getID2, getID3, getID4, action);
                    } else {
                        console.log('Cancel');
                        return false;
                    }
                });
            }

            if (message != '') {
                if (message == 'success') {
                    $("#tr_" + getID).remove();
                }
                Swal.fire({

                });
            }
        }
    });
}

function deleteSearch(getData, action, returnAction) {
    console.log(getData);
    $.ajax({
        type: 'post',
        url: action,
        data: { id: getData },
        success: function (data) {
            console.log('message:' + data.res);
            if (data.res != '') {
                Swal.fire({
                    icon: data.res,
                    title: data.res,
                    text: data.res == "success" ? "Delete Complete" : "Delete not Complete",

                    //title: "Are you sure?",
                    //text: message,
                    //icon: "warning",
                    showConfirmButton: data.res == "success" ? true : false,
                    showCancelButton: data.res == "success" ? false : true,
                    confirmButtonText: "OK",
                    cancelButtonText: "NO"
                }).then((result) => {
                    if (result['isConfirmed']) {
                        console.log('Confirm');
                        console.log(returnAction);
                        window.location.href = returnAction;
                        //Delete(getID, action);
                    } else {
                        console.log('Cancel');
                        return false;
                    }
                });
            }
        }
    });
}
function DeleteDoc(Docno, mode, action, msg) {
    Swal.fire({
        icon: 'warning',
        title: 'warning',
        text: "No data !! ",
    })
}
function ManualRequest() {

    let v_mold_ac = document.getElementById('c_moldAc').value;
    let v_Month_ac = document.getElementById('c_date').value;
    let v_Dep_ac = document.getElementById('c_dept').value;

    if (v_mold_ac == "" || v_Month_ac == "" || v_Dep_ac == "") {
        swal.fire({
            title: 'warning',
            icon: 'warning',
            text: "กรุณากรอกข้อมูลให้ครบถ้วน !!!",

        })
            .then((result) => {
                if (result.isConfirmed) {
                    //var a = "?test=" + config.c1;
                    //// $.get('https://api.ipify.org?format=json')
                    //window.location.href = '@Url.Action("Index", "Home")' + a;
                }

            })
    } else {
        //window.location.href = '@Url.Action("LoadData", "LoadData")' + LoadData;
        window.location.href = '@Url.Action("ManualRequestSheet", "LoadData", new { vMoldActivity = ' + v_mold_ac + ', vdep = ' + v_Dep_ac + ' ,vMonth=' + v_Dep_ac + ' })';
    }



    //$.ajax({
    //    type: 'post',
    //    url: action,
    //    data: "",//{ MoldNo: v_mold_ac, Docno: "-", MoldName: "33", moldday: "", vMonth:"2024/07" },//string MoldNo, string Docno, Class @class, string MoldName, string moldday
    //    success: function (data) {
    //    }
    //});
    //console.log("111111");
}



function sendMail(icount, mode, action) {
    document.getElementById('i_status').removeAttribute("disabled");
    document.getElementById('i_Docno').removeAttribute("disabled");
    console.log("i_status+++>" + document.getElementById('i_status').value);
    console.log("i_Docno+++>" + document.getElementById('i_Docno').value);
    console.log("i_Plant+++>" + document.getElementById('i_Plant').value);
    console.log("i_date+++>" + document.getElementById('i_date').value);
    console.log("icount==> " + icount);
    var id;
    var i_Docno;/*= document.getElementById('i_Docno').value;*/
    var i_Plant; /*= document.getElementById('i_Plant').value;*/
    var i_date; /*= document.getElementById('i_date').value;*/
    //console.log("i_Docno" + i_Docno);
    //if (i_Docno == "") {
    //    id = "";
    //} else {
    //    id = $("#i_Docno").val();
    //}
    id = document.getElementById('i_Docno').value;//$("#i_Docno").val();
    i_Plant = document.getElementById('i_Plant').value;// $("#i_Plant").val();
    i_date = document.getElementById('i_date').value;//$("#i_date").val();
    i_status = document.getElementById('i_status').value; //   $("#i_status").val();
    if (icount == null || icount == "0" || icount == "") {
        document.getElementById('i_status').disabled = true;
        document.getElementById('i_Docno').disabled = true;

        Swal.fire({
            icon: 'warning',
            title: 'warning',
            text: "No data !! ",
        })
    } else {
        $.ajax({
            type: 'post',
            url: action,
            data: { id: id, mode: mode, vplant: i_Plant, vdate: i_date },
            success: function (data) {

                var htmls;
                //console.log(data.lrBuiltDrawing.bdDocumentNo);

                if (data.status == "hasHistory") {
                    htmls = " <div class='panel panel-default property'>"
                    // console.log(data.listHistory.length);
                    $.each(data.listHistory, function (i, item) {
                        //console.log('test' + item.htTo); console.log(data.listHistory[0].htTo);
                        console.log("OK")
                        htmls += "     <div class='panel-heading panel-heading-custom property' tabindex = '0' >"
                        htmls += "         <h4 class='panel-title faq-title-range collapsed' data-bs-toggle='collapse' data-bs-target='#Ans" + item.htStep + "' aria-expanded='false' aria-controls='collapseExample'>"
                        htmls += "             <label style='font-size: smaller;'>Step " + item.htStatus + "</label> <label class='lbV'></label>"
                        htmls += "         </h4>"
                        htmls += "     </div >"
                        htmls += "     <div class='panel-collapse collapse' style = 'overflow: auto;' id = 'Ans" + item.htStep + "' > "

                        htmls += "         <div class='panel-body'>"
                        htmls += "             <div style='font-size: x-small; clear: both; width: 100%; tetx-align: left; font-weight: bold;'>"
                        htmls += "                 <label> " + item.htDate + " :: " + item.htTime + " น.</label>"
                        htmls += "             </div>"
                        htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                        htmls += "                 <label>FROM : </label></br>"
                        htmls += "                 <label>" + item.htFrom + "</label > "
                        htmls += "             </div>"
                        htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                        htmls += "                 <label>TO : </label></br>"
                        htmls += "                 <label>" + item.htTo + "</label>"
                        htmls += "             </div>"
                        htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                        if (item.htCC == null) { item.htCC = "" }
                        else { item.htCC = item.htCC }
                        htmls += "                 <label>CC : </label>"
                        htmls += "                 <label>" + item.htCC + "</label>"
                        htmls += "             </div>"
                        htmls += "             <div style='font-size: x-small; float: right; width: 20%; tetx-align: left;'>"
                        if (item.htRemark == null) { item.htRemark = "" }
                        else { item.htRemark = item.htRemark }
                        htmls += "                 <label>Remark : </label>"
                        htmls += "                 <label>" + item.htRemark + "</label>"
                        htmls += "             </div>"
                        htmls += "             <div style='font-size: x-small; float: right; width: 20%; tetx-align: left;'>"
                        htmls += "                 <label>Status : </label>"
                        if (item.htStatus == null) { item.htStatus = "" }
                        else {
                            item.htStatus = item.htStatus
                            if (item.htStatus == "Finished") {
                                htmls += "                 <label><span style='color: green;'>" + item.htStatus + "</span></label>"
                            } else {
                                htmls += "                 <label><span style='color: darkkhaki;'>" + item.htStatus + "</span></label>"
                            }
                        }

                        htmls += "             </div>"
                        htmls += "         </div>"
                        htmls += "     </div>"

                    });
                    htmls += "</div>"
                }

                var url = data.partial;
                $("#myModalBodyDiv1").load(url, function () {
                    $('#divHistory').append(htmls);
                    $("#myModal1").modal("show");

                })
            }
        });
    }





}

function sendMailRequest(id, mode, action) {
    //let formData = document.forms.namedItem("formManualRequest");
    //let viewModel = new FormData(formData);
    //$.each(formData, function (index, input) {
    //    viewModel.append(input.name, input.value);
    //});


    var mydata = $("#formRequest").serialize();
    $.ajax({
        type: 'post',
        url: action,
        data: mydata,//{ id: id, mode: mode, vplant: "", vdate: "" },


        //data: viewModel,
        //processData: false,
        //contentType: false,


        success: function (data) {

            var htmls;
            //console.log(data.lrBuiltDrawing.bdDocumentNo);

            if (data.status == "hasHistory") {
                htmls = " <div class='panel panel-default property'>"
                // console.log(data.listHistory.length);
                $.each(data.listHistory, function (i, item) {
                    //console.log('test' + item.htTo); console.log(data.listHistory[0].htTo);
                    console.log("OK")
                    htmls += "     <div class='panel-heading panel-heading-custom property' tabindex = '0' >"
                    htmls += "         <h4 class='panel-title faq-title-range collapsed' data-bs-toggle='collapse' data-bs-target='#Ans" + item.htStep + "' aria-expanded='false' aria-controls='collapseExample'>"
                    htmls += "             <label style='font-size: smaller;'>Step " + item.htStatus + "</label> <label class='lbV'></label>"
                    htmls += "         </h4>"
                    htmls += "     </div >"
                    htmls += "     <div class='panel-collapse collapse' style = 'overflow: auto;' id = 'Ans" + item.htStep + "' > "

                    htmls += "         <div class='panel-body'>"
                    htmls += "             <div style='font-size: x-small; clear: both; width: 100%; tetx-align: left; font-weight: bold;'>"
                    htmls += "                 <label> " + item.htDate + " :: " + item.htTime + " น.</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                    htmls += "                 <label>FROM : </label></br>"
                    htmls += "                 <label>" + item.htFrom + "</label > "
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                    htmls += "                 <label>TO : </label></br>"
                    htmls += "                 <label>" + item.htTo + "</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                    if (item.htCC == null) { item.htCC = "" }
                    else { item.htCC = item.htCC }
                    htmls += "                 <label>CC : </label>"
                    htmls += "                 <label>" + item.htCC + "</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: right; width: 20%; tetx-align: left;'>"
                    if (item.htRemark == null) { item.htRemark = "" }
                    else { item.htRemark = item.htRemark }
                    htmls += "                 <label>Remark : </label>"
                    htmls += "                 <label>" + item.htRemark + "</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: right; width: 20%; tetx-align: left;'>"
                    htmls += "                 <label>Status : </label>"
                    if (item.htStatus == null) { item.htStatus = "" }
                    else {
                        item.htStatus = item.htStatus
                        if (item.htStatus == "Finished") {
                            htmls += "                 <label><span style='color: green;'>" + item.htStatus + "</span></label>"
                        } else {
                            htmls += "                 <label><span style='color: darkkhaki;'>" + item.htStatus + "</span></label>"
                        }
                    }

                    htmls += "             </div>"
                    htmls += "         </div>"
                    htmls += "     </div>"

                });
                htmls += "</div>"
            }

            var url = data.partial;
            console.log("url" + url); //myModal myModalBodyDiv
            $("#myModalBodyDiv").load(url, function () {
                console.log($('#divHistory'));
                //console.log($('#myModal'));
                $('#divHistory').append(htmls);
                $("#myModal").modal("show");

            });
        }
    });


}

function sendMailRequestManual(id, mode, action) {
    let formData = document.forms.namedItem("formManualRequest");
    let viewModel = new FormData(formData);
    $.each(formData, function (index, input) {
        viewModel.append(input.name, input.value);
    });


    var mydata = $("#formRequest").serialize();
    $.ajax({
        type: 'post',
        url: action,
        //data: mydata,//{ id: id, mode: mode, vplant: "", vdate: "" },


        data: viewModel,
        processData: false,
        contentType: false,


        success: function (data) {

            var htmls;
            //console.log(data.lrBuiltDrawing.bdDocumentNo);

            if (data.status == "hasHistory") {
                htmls = " <div class='panel panel-default property'>"
                // console.log(data.listHistory.length);
                $.each(data.listHistory, function (i, item) {
                    //console.log('test' + item.htTo); console.log(data.listHistory[0].htTo);
                    console.log("OK")
                    htmls += "     <div class='panel-heading panel-heading-custom property' tabindex = '0' >"
                    htmls += "         <h4 class='panel-title faq-title-range collapsed' data-bs-toggle='collapse' data-bs-target='#Ans" + item.htStep + "' aria-expanded='false' aria-controls='collapseExample'>"
                    htmls += "             <label style='font-size: smaller;'>Step " + item.htStatus + "</label> <label class='lbV'></label>"
                    htmls += "         </h4>"
                    htmls += "     </div >"
                    htmls += "     <div class='panel-collapse collapse' style = 'overflow: auto;' id = 'Ans" + item.htStep + "' > "

                    htmls += "         <div class='panel-body'>"
                    htmls += "             <div style='font-size: x-small; clear: both; width: 100%; tetx-align: left; font-weight: bold;'>"
                    htmls += "                 <label> " + item.htDate + " :: " + item.htTime + " น.</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                    htmls += "                 <label>FROM : </label></br>"
                    htmls += "                 <label>" + item.htFrom + "</label > "
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                    htmls += "                 <label>TO : </label></br>"
                    htmls += "                 <label>" + item.htTo + "</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: left; width: 20%; tetx-align: left;'>"
                    if (item.htCC == null) { item.htCC = "" }
                    else { item.htCC = item.htCC }
                    htmls += "                 <label>CC : </label>"
                    htmls += "                 <label>" + item.htCC + "</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: right; width: 20%; tetx-align: left;'>"
                    if (item.htRemark == null) { item.htRemark = "" }
                    else { item.htRemark = item.htRemark }
                    htmls += "                 <label>Remark : </label>"
                    htmls += "                 <label>" + item.htRemark + "</label>"
                    htmls += "             </div>"
                    htmls += "             <div style='font-size: x-small; float: right; width: 20%; tetx-align: left;'>"
                    htmls += "                 <label>Status : </label>"
                    if (item.htStatus == null) { item.htStatus = "" }
                    else {
                        item.htStatus = item.htStatus
                        if (item.htStatus == "Finished") {
                            htmls += "                 <label><span style='color: green;'>" + item.htStatus + "</span></label>"
                        } else {
                            htmls += "                 <label><span style='color: darkkhaki;'>" + item.htStatus + "</span></label>"
                        }
                    }

                    htmls += "             </div>"
                    htmls += "         </div>"
                    htmls += "     </div>"

                });
                htmls += "</div>"
            }

            var url = data.partial;
            console.log("url" + url); //myModal myModalBodyDiv
            $("#myModalBodyDiv").load(url, function () {
                console.log($('#divHistory'));
                //console.log($('#myModal'));
                $('#divHistory').append(htmls);
                $("#myModal").modal("show");

            });
        }
    });


}


function CheckDis(status) {
    $(document).ready(function () {
        var checkStatusDis = status;
        var step = $('#step').val();
        console.log("step ==> " + step);
        if (checkStatusDis == 'Disapprove') {
            $('#searchInputTO').attr("disabled", "disabled");
            document.getElementById("searchInputTO").value = "";
            $('#EmailTo').removeAttr("name");
        }
        else {
            $('#searchInputTO').removeAttr('disabled', 'disabled');
            $('#EmailTo').removeAttr("name");
            if (step == 4) {
                console.log(step);
                document.getElementById("searchInputTO").value = $('#EmailTo').val();
            }

        }

    });
}

function editPage(getID, action) {
    $.ajax({
        type: 'post',
        url: action,
        data: { DocumentNo: getID },
        success: function (page) {
            console.log(page.action);
            window.location.href = page.action;
        }
    });
}

function editPageSearch(getID, action) {
    $.ajax({
        type: 'post',
        url: action,
        data: { DocumentNo: getID },
        success: function (page) {
            console.log(page.action);
            window.location.href = page.action;
        }
    });
}



//capture
function capture() {
    html2canvas(document.body, { useCORS: true, scrollY: -window.scrollY }).then(canvas => {
        let link = document.createElement('a');
        link.download = 'screenshot.png';
        link.href = canvas.toDataURL();
        link.click();
    });
}

