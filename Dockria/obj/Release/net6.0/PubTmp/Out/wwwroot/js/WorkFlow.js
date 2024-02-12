var dataTable = null;
let firstLoad = true;
fetchDropdownOptions('#groupList');

// this function for load all details into Index view of Document Type....
loadDataTable();


function loadDataTable() {
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/Document/GetAll",
            "type": "GET",
            "dataType": "json",
            "error": function (xhr, status, error) {
                debugger;
                console.error("AJAX Error:", error);
            }
        },
        "columns": [
            {
                "data": "docTypeName",
                "width": "30%",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "ocr",
                "width": "10%",
                "render": function (data) {
                    if (data) {
                        return '<div class="expandable-cell">Enable</div>';
                    } else {
                        return '<div class="expandable-cell">Disable</div>';
                    }
                }
            },
            {
                "data": "autoFileName",
                "width": "40%",
                "render": function (data) {
                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "versioning",
                "width": "10%",
                "render": function (data) {
                    if (data) {
                        return '<div class="expandable-cell">Enable</div>';
                    } else {
                        return '<div class="expandable-cell">Disable</div>';
                    }
                }
            },
            {
                "data": "id",
                "width": "10%",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action text-center">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" data-toggle="tooltip" title="Edit Document Type"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editUserModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick=editDocumentType("/CompanyAdmin/Document/UpdateDocument/${data}")>`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" data-toggle="tooltip" title="Delete Document Type"`;
                    html += `data-original-title="Remove" data-bs-toggle="modal" data-bs-target="#deleteUserModal"`;
                    html += `data-user-id="${data}" data-action="delete" onclick=deleteDocumentType("/CompanyAdmin/Document/DeleteDocument/${data}")>`;
                    html += `<i class="fa fa-trash"></i></button>`;

                    html += `</div>`;
                    return html;
                }
            }
        ]
    })
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
}
