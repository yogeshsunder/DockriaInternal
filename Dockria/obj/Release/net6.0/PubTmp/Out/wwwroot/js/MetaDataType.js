var dataTable = null;

$(document).ready(function () {
    debugger;
    loadDataTable();
    // this is for show dropdown list showing of UserGrops
    fetchDropdownOptions('#groupList');
    fetchDropdownOptions('#metaGroupList');

    // Call the function to handle search input focus and blur
    handleSearchInputFocusBlur();

    $('#addMetaDataTypeModal').on('hidden.bs.modal', function () {
        debugger;
        var form = $("#metadata-form");
        // Clear the form
        form[0].reset();
    });
});

// Function to handle search input focus and blur
function handleSearchInputFocusBlur() {
    $(document).on('focus', '.dataTables_wrapper input[type="search"]', function () {
        // Hide the icon when input is focused
        $(this).closest('.dataTables_wrapper').addClass('search-focused');
    });

    $(document).on('blur', '.dataTables_wrapper input[type="search"]', function () {
        // Show the icon when input loses focus
        $(this).closest('.dataTables_wrapper').removeClass('search-focused');
    });
}

function fetchDropdownOptions(selector, selectedOptions) {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/Document/ShowUserList',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            console.log(data.userlist);
            populateDropdown(selector, data.userlist, selectedOptions);
        },
        error: function (error) {
            console.error(error.statusText);
        }
    });
}

// This function for populate all userGroup list into dropdown of document popup view
function populateDropdown(selector, options, selectedOptions) {
    debugger;
    var dropdown = $(selector);
    dropdown.empty();
    dropdown.append('<option disabled selected>Select an option</option>');

    // Populate options
    $.each(options, function (index, option) {
        debugger;
        dropdown.append($('<option>', {
            value: option.value,
            text: option.text
        }));
    });

    // If selectedOptions is provided, set the selected options
    if (selectedOptions) {
        debugger;
        for (var i = 0; i < dropdown[0].options.length; i++) { // Use 'dropdown' instead of 'userGroupsSelect'
            debugger;
            if (selectedOptions.includes(dropdown[0].options[i].value)) {
                dropdown[0].options[i].selected = true;
            } else {
                dropdown[0].options[i].selected = false;
            }
        }
        dropdown.val(selectedOptions.map(option => option.value));
    }

    // Refresh the Bootstrap Selectpicker, if applicable
    if (dropdown.hasClass('selectpicker')) {
        dropdown.selectpicker('refresh');
    }
}

// This Function for AddFileTypeMetadata "POST" method...
function addMetadata() {
    debugger;
    var form = $("#metadata-form");

    // Clear existing error messages
    $(".error-message").empty();
    var isValid = true;

    // Validation for Document Type Name
    var metaDataName = $("#metadata-name").val();
    if (!metaDataName) {
        $("#metaDataNameError").text("MetaData Name is required.");
        isValid = false;
    }

    // Clear the Document Type Name error message when input changes
    $("#metadata-name").on("input", function () {
        $("#metaDataNameError").empty();
    });

    // Add event listener to metadata type dropdown
    $("#metadataType").on("change", function () {
        var selectedValue = $(this).val();
        if (!selectedValue) {
            $("#metaDataTypeError").text("METADATA DATA TYPE is required.");
        } else {
            $("#metaDataTypeError").empty();
        }
    });

    // Validation for User Group
    var selectedUserGroups = $("#groupList option:selected:not([disabled])").length;
    if (selectedUserGroups === 0) {
        $("#UserGroupError").text("At least one User Group must be selected.");
        isValid = false;
    }

    // Clear the User Group error message when a new option is selected
    $("#groupList").on("change", function () {
        $("#UserGroupError").empty();
    });

    if (!isValid) {
        return; // Stop the process if validation fails
    }

    // Get selected user group list from multi-select dropdown
    var selectedUserGroups = [];
    $("#groupList option:selected").each(function () {
        selectedUserGroups.push($(this).val());
    });

    // Retrieve metadata name and type
    var metaDataTypeName = $("#metadata-name").val();
    var metaDataDataType = $("#metadataType").val();

    // Combine the form data with the Selected GroupNames and metadata info
    var formData = new FormData(form[0]);
    formData.append("SelectedUserGroups", JSON.stringify(selectedUserGroups));
    formData.append("MetaDataTypeName", metaDataTypeName); // Use "MetaDataTypeName" as key
    formData.append("MetaDataDataType", metaDataDataType);

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: formData,
        dataType: "json",
        contentType: false, // Important for sending FormData
        processData: false, // Important for sending FormData
        success: function (response) {
            debugger;
            if (response.success) {
                debugger;
                // Clear the form
                form[0].reset();
                // Hide the modal
                $("#addMetaDataTypeModal").modal("hide");
                // do something (e.g., show a success message)
                not1(response.message);
                dataTable.ajax.reload();
            } else {
                // Document save failed, handle the error (e.g., show an error message)
                console.error("Document save failed:", response.errorMessage);
            }
        },
        error: function (error) {
            console.error("An error occurred:", error);
        }
    });
}

// this function for show all MetaData into index main page...
function loadDataTable() {
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/Document/GetAllMetaDataType",
            "type": "GET",
            "dataType": "json",
            "error": function (xhr, status, error) {
                debugger;
                console.error("AJAX Error:", error);
            }
        },
        "columns": [
            {
                "data": "metadataName",
                "width": "20%",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "metaDatatype",
                "width": "20%",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "userGroup",
                "width": "20%",
                "render": function (data) {
                    var groupNames = data.map(function (group) {
                        return group.groupName;
                    }).join(', ');

                    return '<div class="expandable-cell">' + groupNames + '</div>';
                }
            },
            {
                "data": "id",
                "width": "10%",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action text-center">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" data-toggle="tooltip" title="Edit MetaData Type"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editMetaDataTypeModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick=editMetaDataType("/CompanyAdmin/Document/EditMetaDataType/${data}")>`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" data-toggle="tooltip" title="Delete MetaData Type"`;
                    html += `data-original-title="Remove" data-bs-toggle="modal" data-bs-target="#deleteUserModal"`;
                    html += `data-action="delete" onclick=deleteMetaDataType("/CompanyAdmin/Document/DeleteMetaData/${data}")>`;
                    html += `<i class="fa fa-trash"></i></button>`;

                    html += `</div > `;
                    return html;
                }
            }
        ],
        "order": [[3, "desc"]]
    })
}

function deleteMetaDataType(url) {
    debugger;

    // Show a confirmation dialog using SweetAlert
    swal({
        title: 'Do you want to delete this MetaData ?',
       
        icon: 'warning',
        buttons: {
            delete: {
                text: 'Delete',
                value: true,
                dangerMode: true,
            },
            cancel: 'Cancel',
        },
    }).then(function (confirmed) {
        if (confirmed) {
            // Perform the AJAX request
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    // Handle the response from the server
                    if (response) {
                        toastr.success(response.message);
                        // Reload the page after a delay of 2 seconds
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                },
                error: function (error) {
                    // Handle AJAX error
                    // ...
                }
            });
        }
    });
}

// This function for Edit MetaDataType "GET" method
function editMetaDataType(url) {
    debugger;
    //var form = $("#editFileTypeForm");
    var modal = $('#editMetaDataTypeModal');

    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            debugger;
            // Handle the response from the server
            if (response.success) {
                debugger;
                var fileMetaDataType = response.data;
                $('#id').val(fileMetaDataType.id);
                // Populate the filename input field
                $('#metaDataTypeName').val(fileMetaDataType.metaDataName);
                // Set selected value for metadata data type dropdown
                $("#metaDataDataType").val(fileMetaDataType.metaDataType);
                // Assuming you have retrieved the user group names array from the response
                var selectedUserGroupNames = fileMetaDataType.userGroups;

                // Fetch and populate the dropdown options for user groups
                fetchDropdownOptions('#userGroupNames', selectedUserGroupNames);
                // Show the modal
                modal.modal('show');
            } else {
                toastr.error(data.message);
            }
        },
        error: function (error) {
            debugger;
            alert(error);
            // Handle AJAX error
            // ...
        }
    });
}

// This function for Edit MetaData Type "POST" Method
function updateMetadata() {
    debugger;
    var form = $("#updateMetaData-form");

    // Clear existing error messages
    $(".error-message").empty();
    var isValid = true;

    // Validation for Document Type Name
    var metaDataName = $("#metaDataTypeName").val();
    if (!metaDataName) {
        $("#metaDataNameError_1").text("MetaData Name is required.");
        isValid = false;
    }

    // Clear the Document Type Name error message when input changes
    $("#metaDataTypeName").on("input", function () {
        $("#metaDataNameError_1").empty();
    });

    // Add event listener to metadata type dropdown
    $("#metaDataDataType").on("change", function () {
        var selectedValue = $(this).val();
        if (!selectedValue) {
            $("#metaDataTypeError_1").text("METADATA DATA TYPE is required.");
        } else {
            $("#metaDataTypeError_1").empty();
        }
    });

    // Validation for User Group
    var selectedUserGroups = $("#userGroupNames option:selected:not([disabled])").length;
    if (selectedUserGroups === 0) {
        $("#UserGroupsError_1").text("At least one User Group must be selected.");
        isValid = false;
    }

    // Clear the User Group error message when a new option is selected
    $("#userGroupNames").on("change", function () {
        $("#UserGroupsError_1").empty();
    });

    if (!isValid) {
        return; // Stop the process if validation fails
    }


    // Get selected user group list from multi-select dropdown
    var selectedUserGroups = [];
    $("#userGroupNames option:selected").each(function () {
        selectedUserGroups.push($(this).val());
    });

    // Combine the form data with the Selected GroupNames and metadata info
    var formData = new FormData(form[0]);
    formData.append("SelectedUserGroups", JSON.stringify(selectedUserGroups));

    $.ajax({
        url: form.attr("action"),
        type: "POST",
        data: formData,
        dataType: "json",
        contentType: false, // Important for sending FormData
        processData: false, // Important for sending FormData
        success: function (response) {
            debugger;
            if (response.success) {
                debugger;
                // Clear the form
                form[0].reset();
                // Reset the form fields inside the #addDocumentModal
                //resetAddDocumentModal();
                // Hide the modal
                $("#editMetaDataTypeModal").modal("hide");
                // do something (e.g., show a success message)
                not1(response.message);
                dataTable.ajax.reload();
            } else {
                // Document save failed, handle the error (e.g., show an error message)
                console.error("Document save failed:", response.errorMessage);
            }
        },
        error: function (error) {
            console.error("An error occurred:", error);
        }
    });
}

// this function for showing user define alert message .....
function not1(message) {
    debugger;
    notif({
        type: "success",
        msg: message,
        position: "right"
    });
};

// Download All MetaDataType in Excel Format...
function exportExcel() {
    debugger;
    window.location.href = '/CompanyAdmin/Document/ExportExcelMetaData';
}

// Download All Metadata Type in PDF Format...
function exportPdf() {
    debugger;
    window.location.href = '/CompanyAdmin/Document/ExportPdfMetaDataType';
}