var dataTable = null; // Declare the dataTable variable
$(document).ready(function () {
    // show all data into table
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/GroupPermission/GetAll"
        },
        "columns": [
            { "data": "groupName", "width": "10%", "className": "text-center" },
            {
                "data": "userId",
                "width": "10%",
                "render": function (data) {
                    debugger;
                    var html = `<div class="button-container">`;

                    html += `<button class="btn btn-link btn-simple-primary btn-sm btn-edit" data-toggle="tooltip" title="Edit DMS"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editDmsModal" data-user-id="${data}"`;
                    html += `data-action="edit" data-bs-placement="top" onclick=editDMS("/CompanyAdmin/GroupPermission/GetDMSDetails/${data}")>`;
                    html += `DMS Edit</button>`;

                    html += `<button class="btn btn-link btn-simple-primary btn-sm btn-edit" data-toggle="tooltip" title="Edit EndUser"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editEndUserModal" data-user-id="${data}"`;
                    html += ` data-action="edit" data-bs-placement="top" onclick=editEndUser("/CompanyAdmin/GroupPermission/GetEndUserDetails/${data}")>`;
                    html += `End User Edit</button>`;

                    html += `<button class="btn btn-link btn-simple-primary btn-sm btn-edit" data-toggle="tooltip" title="Edit RAD"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editRadModal" data-user-id="${data}"`;
                    html += `data-action="edit" data-bs-placement="top" onclick=editRad("/CompanyAdmin/GroupPermission/GetRadDetails/${data}")>`;
                    html += `RAD Edit</button>`;

                    html += `</div>`;
                    return html;
                }
            }
        ]
    });

    // AJAX POST request to submit the DMS Managment Permission form data
    $(document).on("click", "#updateDmsSubmitButton", function () {
        debugger;

        var dmsData = {
            Id: $('#Id').val(),
            ViewDoc: $('#viewDoc').is(':checked'),
            DocSinAdd: $('#docSinAdd').is(':checked'),
            DocMulAdd: $('#docMulAdd').is(':checked'),
            DocCopy: $('#docCopy').is(':checked'),
            DocMove: $('#docMove').is(':checked'),
            DocDelete: $('#docDelete').is(':checked'),
            DocRename: $('#docRename').is(':checked'),
            DocPrivate: $('#docPrivate').is(':checked'),
            DocDown: $('#docDown').is(':checked'),
            DocPrint: $('#docPrint').is(':checked'),
            ViewMatadata: $('#viewMatadata').is(':checked'),
            EditMatadata: $('#editMatadata').is(':checked'),
            ShareDocInt: $('#shareDocInt').is(':checked'),
            ShareDocExt: $('#shareDocExt').is(':checked'),
            ShareSigExt: $('#shareSigExt').is(':checked'),
            AuditLogDoc: $('#auditLogDoc').is(':checked'),
            DocVerView: $('#docVerView').is(':checked'),
            DocRollBack: $('#docRollBack').is(':checked'),
            DownCsvRpt: $('#downCsvRpt').is(':checked'),
            AuditLogUser: $('#auditLogUser').is(':checked'),
            AsgnDocUser: $('#asgnDocUser').is(':checked'),
            MaxDocUpSize: $('#maxDocUpSize').is(':checked'),
            MaxDocUpNum: $('#maxDocUpNum').val(),
        };

        $.ajax({
            url: "/CompanyAdmin/GroupPermission/UpdateDms", // Specify the URL to the action method
            type: "POST",
            data: dmsData,
            //contentType: "application/json",
            success: function (response) {
                debugger;
                // Close the modal
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
                toastr.success(response.message);
                $("#editDmsModal").modal("hide");
                
                //dataTable.ajax.reload(null, false);

            },
            error: function (error) {
                // Handle the error response
                console.log(error);
                // Display an error message or handle the error condition
            }
        });
    });

    // AJAX POST request to submit the End User Permission form data
    $(document).on("click", "#endUserSubmitButton", function () {
        debugger;

        var endUserData = {
            Id: $('#endUserId').val(),
            EditEmail: $('#editEmail').is(':checked'),
            EditPassword: $('#editPassword').is(':checked'),
            EditSign: $('#editSign').is(':checked'),
        };

        $.ajax({
            url: "/CompanyAdmin/GroupPermission/UpdateEndUserDetails", // Specify the URL to the action method
            type: "POST",
            data: endUserData,
            //contentType: "application/json",
            success: function (response) {
                debugger;
                setTimeout(function () {
                    window.location.reload();
                }, 2000);
                console.log(response);
                // Close the modal
                toastr.success(response.message);
                $("#editEndUserModal").modal("hide");
                // Perform any additional actions or updates on the page
                
            },
            error: function (error) {
                // Handle the error response
                console.log(error);
                // Display an error message or handle the error condition
            }
        });
    });

    // AJAX POST request to submit the RAD Permission form data
    $(document).on("click", "#updateRadSubmitButton", function () {
        debugger;

        var radData = {
            Id: $('#editRadId').val(),
            RadView: $('#radView').is(':checked'),
            RadEdit: $('#radEdit').is(':checked'),
            RadFormFill: $('#radFormFill').is(':checked'),
        };

        $.ajax({
            url: "/CompanyAdmin/GroupPermission/UpdateRadDetails", // Specify the URL to the action method
            type: "POST",
            data: radData,
            //contentType: "application/json",
            success: function (response) {
                debugger;
                setTimeout(function () {
                    window.location.reload();
                }, 2000);

                console.log(response);
                // Close the modal
                toastr.success(response.message);
                $("#editRadModal").modal("hide");
                // Perform any additional actions or updates on the page
            },
            error: function (error) {
                // Handle the error response
                console.log(error);
                // Display an error message or handle the error condition
            }
        });
    });

    $('.dataTables_wrapper input[type="search"]').on('focus', function () {
        debugger;
        // Add a class to hide the icon when input is focused
        $('.dataTables_wrapper').addClass('search-focused');
    });

    $('.dataTables_wrapper input[type="search"]').on('blur', function () {
        debugger;
        // Remove the class to show the icon when input loses focus
        $('.dataTables_wrapper').removeClass('search-focused');
    });
})

function editDMS(url) {
    debugger;

    // Make the AJAX request to fetch the data
    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            debugger;
            if (data.status === true) {
                // Update the popup view with the fetched data
                updateModal(data);
            }
            else {
                // Clear the checkboxes in the popup view
                clearModal();
            }
        },
        error: function (xhr, status, error) {
            // Handle the error scenario
            console.log(error);
        }
    });
}
function editEndUser(url) {
    debugger;

    // Make the AJAX request to fetch the data
    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            debugger;
            if (data.status === true) {
                // Update the popup view with the fetched data
                updateModal(data);
            }
            else {
                // Clear the checkboxes in the popup view
                clearModal();
            }
        },
        error: function (xhr, status, error) {
            // Handle the error scenario
            console.log(error);
        }
    });
}
function editRad(url) {
    debugger;
    // Make the AJAX request to fetch the data
    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            debugger;
            if (data.status === true) {
                // Update the popup view with the fetched data
                updateModal(data);
            }
            else {
                // Clear the checkboxes in the popup view
                clearModal();
            }
        },
        error: function (xhr, status, error) {
            // Handle the error scenario
            console.log(error);
        }
    });
}

function updateModal(data) {
    debugger;
    if (data.status === true) {
        var radData = data.data; // Access the RADManagement object

        // Check if the response is from the GetRadDetails action
        if (radData.hasOwnProperty('radView')) {
            debugger;
            $('#editRadId').val(radData.id);
            $("#editRadForm #radView").prop("checked", radData.radView);
            $("#editRadForm #radEdit").prop("checked", radData.radEdit);
            $("#editRadForm #radFormFill").prop("checked", radData.radFormFill);
        }

        // Check if the response is from the GetEndUserDetails action
        if (radData.hasOwnProperty('editEmail')) {
            debugger;
            $('#endUserId').val(radData.id);
            $("#editEndUserForm #editEmail").prop("checked", radData.editEmail);
            $("#editEndUserForm #editPassword").prop("checked", radData.editPassword);
            $("#editEndUserForm #editSign").prop("checked", radData.editSign);
        }

        // Check if the response is from the GetEndUserDetails action
        if (radData.hasOwnProperty('viewDoc')) {
            debugger;
            $('#Id').val(radData.id);
            $("#editDmsForm #viewDoc").prop("checked", radData.viewDoc);
            $("#editDmsForm #docSinAdd").prop("checked", radData.docSinAdd);
            $("#editDmsForm #docMulAdd").prop("checked", radData.docMulAdd);
            $("#editDmsForm #docCopy").prop("checked", radData.docCopy);
            $("#editDmsForm #docMove").prop("checked", radData.docMove);
            $("#editDmsForm #docDelete").prop("checked", radData.docDelete);
            $("#editDmsForm #docRename").prop("checked", radData.docRename);
            $("#editDmsForm #docPrivate").prop("checked", radData.docPrivate);
            $("#editDmsForm #docDown").prop("checked", radData.docDown);
            $("#editDmsForm #docPrint").prop("checked", radData.docPrint);
            $("#editDmsForm #viewMatadata").prop("checked", radData.viewMatadata);
            $("#editDmsForm #editMatadata").prop("checked", radData.editMatadata);
            $("#editDmsForm #shareDocInt").prop("checked", radData.shareDocInt);
            $("#editDmsForm #shareDocExt").prop("checked", radData.shareDocExt);
            $("#editDmsForm #shareSigExt").prop("checked", radData.shareSigExt);
            $("#editDmsForm #auditLogDoc").prop("checked", radData.auditLogDoc);
            $("#editDmsForm #docVerView").prop("checked", radData.docVerView);
            $("#editDmsForm #docRollBack").prop("checked", radData.docRollBack);
            $("#editDmsForm #downCsvRpt").prop("checked", radData.downCsvRpt);
            $("#editDmsForm #auditLogUser").prop("checked", radData.auditLogUser);
            $("#editDmsForm #asgnDocUser").prop("checked", radData.asgnDocUser);
            $("#editDmsForm #maxDocUpSize").prop("checked", radData.maxDocUpSize);
            $("#editDmsForm #maxDocUpNum").val(radData.maxDocUpNum);
        }
    }
}

function clearModal() {
    debugger;
    // Uncheck all checkboxes in the popup view
    $("#editEndUserForm #editEmail").prop("checked", false);
    $("#editEndUserForm #editPassword").prop("checked", false);
    $("#editEndUserForm #editSign").prop("checked", false);
}

// Download Excel File
function exportExcel() {
    debugger;
    window.location.href = '/CompanyAdmin/GroupPermission/ExportExcel';
}

// Download PDF File
function exportPdf() {
    debugger;
    // Show the tooltip
    const exportPdfBtn = $("#exportPdfBtn");
    exportPdfBtn.tooltip("show");

    // Wait for 2 seconds (adjust the delay as needed)
    setTimeout(function () {
        // Hide the tooltip
        exportPdfBtn.tooltip("hide");

        // Initiate the PDF export
        window.location.href = '/CompanyAdmin/GroupPermission/ExportPdf';
    }, 2000); // 2 seconds delay
}
