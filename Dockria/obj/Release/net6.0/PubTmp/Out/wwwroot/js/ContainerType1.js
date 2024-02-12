var dataTable = null;
// Check if the page is loaded for the first time
let firstLoad = true;
// Define a global variable to keep track of added metadata
var addedMetadata = [];
var rowIndex = 0;
var metadataModalOpen = false;
$(document).ready(function () {
    const searchInput = $('#metadataSearchInput');
    const updateSearchInput = $('#updateMetaDataSearchInput');
    const searchIcon = $('#searchIcon');
    const updateSearchIcon = $('#updateSearchIcon');

    // Call the function to handle search input focus and blur
    handleSearchInputFocusBlur();
    // Add a focusin event listener to the input field
    searchInput.on('focusin', function () {
        debugger;
        searchIcon.css('display', 'none');
    });
    updateSearchInput.on('focusin', function () {
        debugger;
        updateSearchIcon.css('display', 'none');
    });

    // Add a focusout event listener to the input field
    searchInput.on('focusout', function () {
        debugger;
        searchIcon.css('display', 'block');
    });
    updateSearchInput.on('focusout', function () {
        debugger;
        updateSearchIcon.css('display', 'block');
    });
    debugger;
    // Function to generate and populate unique ID
    GenerateUniqueId();

    // this is for show dropdown list showing of UserGrops
    fetchDropdownOptions('#groupList');

    loadDataTable();

    // this function for show Metadata PopUp view and Hide Add Container Type popUp view
    $('#metadata-modal').on('show.bs.modal', function (event) {
        debugger;
        var addDocumentModal = document.getElementById('addContainerTypeModal');
        var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
        bootstrapAddDocumentModal.hide();
        // Set the flag to true when metadata modal is opened
        metadataModalOpen = true;
        // Get selected metadata list from the metadata table
        var selectedMetadata = [];
        $(".Meta-Data tr").each(function () {
            var metadataName = $(this).attr("data-metadata-name");
            var isChecked = $(this).find("input[type='checkbox']").prop("checked");
            selectedMetadata.push({ metadataName: metadataName, isRequired: isChecked });
        });

        if (selectedMetadata.length == 0) {
            var currentDateTime = getCurrentDateTime();
            selectedMetadata.push({ name: currentDateTime, isChecked: true });
        }
        fetchMetadata('metaDataBody', selectedMetadata);
    });

    // this function for show UPDATE Case Metadata PopUp view and Hide Update Container Type popUp view
    $('#updateMetadata-modal').on('show.bs.modal', function (event) {
        var updateDocumentModal = document.getElementById('updateContainerTypeModal');
        var bootstrapUpdateDocumentModal = bootstrap.Modal.getInstance(updateDocumentModal);
        bootstrapUpdateDocumentModal.hide();
        debugger;
        var selectedMetadata = [];
        var rows = document.querySelectorAll('.Update-Meta-Data tr');
        rows.forEach(function (row) {
            var metadataName = row.querySelector('.col-8').textContent;
            var isChecked = row.querySelector("input[type='checkbox']").checked;
            selectedMetadata.push({ metadataName: metadataName, isChecked: isChecked });
        });
        fetchMetadata('updateMetaDataTableBody', selectedMetadata);
    });

    // Add event listener to the "SAVE" button
    $(document).on('click', '#addContainerTypeSubmitButton', function () {
        debugger;
        //var selectedMetadata = getSelectedMetadata();
        saveContainerType();
    });






    // Handle search input changes of Add New Document Type MetaData List
    $("#metadataSearchInput").on("input", function () {
        // Fetch the search input value and filter metadata
        var searchValue = $(this).val().toLowerCase();
        filterMetadata('add', searchValue);
    });

    // Handle search input changes of Update Document Type MetaData List
    $("#updateMetaDataSearchInput").on("input", function () {
        // Fetch the search input value and filter metadata
        var searchValue = $(this).val().toLowerCase();
        filterMetadata('update', searchValue);
    });

    // When the "Add-MetaData-modal" is hidden, show the "Update File Type Modal" modal
    //$('#metadata-modal').on('hidden.bs.modal', function (event) {
    //    debugger;
    //    // Reset the flag when metadata modal is closed
    //    metadataModalOpen = false;
    //    // Show the addContainerTypeModal only if metadata modal is closed
    //    if (!metadataModalOpen) {
    //        showAddContainerModal();
    //    }
    //});


    $('#metadata-modal').on('hidden.bs.modal', function (event) {
        debugger;

        // Store the current search value
        lastSearchValue = $('#metadataSearchInput').val();

        // Clear the search input and trigger input event
        $('#metadataSearchInput').val('').trigger('input');

        // Show the addContainerTypeModal only if metadata modal is closed
        showAddContainerModal();
    });
    $('#viewAllValueModal').on('hidden.bs.modal', function (event) {
        debugger;
        // Reset the flag when metadata modal is closed
        metadataModalOpen = false;
        // Show the addContainerTypeModal only if metadata modal is closed
        if (!metadataModalOpen) {
            showAddContainerModal();
        }
    });


    $('#editviewAllValueModal').on('hidden.bs.modal', function (event) {
        debugger;
        // Reset the flag when metadata modal is closed
        metadataModalOpen = false;
        // Show the addContainerTypeModal only if metadata modal is closed
        if (!metadataModalOpen) {
            showeditContainerModal();
        }
    });
    // When the "Update-MetaData-modal" is hidden, show the "Update File Type Modal" modal
    //$('#updateMetadata-modal').on('hidden.bs.modal', function (event) {
    //    debugger;
    //    showUpdateContainerModal();
    //});

    $('#updateMetadata-modal').on('hidden.bs.modal', function (event) {
        debugger;

        // Store the current search value
        lastSearchValue = $('#updateMetaDataSearchInput').val();

        // Clear the search input and trigger input event
        $('#updateMetaDataSearchInput').val('').trigger('input');

        // Show the addContainerTypeModal only if metadata modal is closed
        showUpdateContainerModal();
    });
    $('#addContainerTypeModal').on('hidden.bs.modal', function () {
        debugger;
        // Reset the addContainerTypeModal only if metadata modal is not open
        if (!metadataModalOpen) {
            resetContainerTypeModal();
        }
    });

    $('#openAddMetaDataTypeModalBtn').click(function () {
        debugger;
        $.get("/CompanyAdmin/ContainerType/ShowMetaDataForm", function (data) {
            debugger;
            $('#partialViewContainer').html(data);
            $('#addMetaDataTypeModal').modal('show');
            fetchDropdownOptions('#metaGroupList');
        });
    });
});

// Call the function to set the initial values on page load
setInitialValues();

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
function filterMetadata(context, searchValue) {
    debugger;
    var metaDataTableId = (context === 'add') ? '#metaDataBody' : '#updateMetaDataTableBody';
    var metadataRows = $(metaDataTableId).find("tr");

    metadataRows.each(function () {
        var metadataName = $(this).find("td:first").text().toLowerCase();
        if (metadataName.includes(searchValue)) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });
}

// This Function for Show Add New Container Type PopUp View Modal...
function showAddContainerModal() {
    debugger;
    var addDocumentModal = document.getElementById('addContainerTypeModal');
    var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
    bootstrapAddDocumentModal.show();
}

// This Function for Show Update Container Type PopUp View Modal...
function showUpdateContainerModal() {
    debugger;
    var updateFileNameTypeModal = document.getElementById('updateContainerTypeModal');
    var bootStrapEditModal = bootstrap.Modal.getInstance(updateFileNameTypeModal);
    bootStrapEditModal.show();
}

function showeditContainerModal() {
    debugger;
    var addDocumentModal = document.getElementById('updateContainerTypeModal');
    var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
    bootstrapAddDocumentModal.show();
}

// This Function for Show Update Container Type PopUp View Modal...
function showUpdateContainerModal() {
    debugger;
    var updateFileNameTypeModal = document.getElementById('updateContainerTypeModal');
    var bootStrapEditModal = bootstrap.Modal.getInstance(updateFileNameTypeModal);
    bootStrapEditModal.show();
}
function loadDataTable() {
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/ContainerType/GetAll",
            "type": "GET",
            "dataType": "json",
            "error": function (xhr, status, error) {
                debugger;
                console.error("AJAX Error:", error);
            }
        },
        "columns": [
            {
                "data": "containerName",
                "render": function (data) {
                    return '<div class="text-truncate">' + data + '</div>';
                }
            },
            //{
            //    "data": "userGroup",
            //    "render": function (data) {
            //        return '<div class="text-truncate">' + data + '</div>';
            //    }
            //  },
            {
                "data": "id",
                "width": "10%",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action text-center">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" data-toggle="tooltip" title="Edit Container Type"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#updateContainerTypeModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick=editContainerType("/CompanyAdmin/ContainerType/Update/${data}")>`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" data-toggle="tooltip" title="Delete Container Type"`;
                    html += `data-original-title="Remove" data-bs-toggle="modal" data-bs-target="#deleteContainerModal"`;
                    html += `data-user-id="${data}" data-action="delete" onclick=deleteContainerType("/CompanyAdmin/ContainerType/DeleteContainer/${data}")>`;
                    html += `<i class="fa fa-trash"></i></button>`;

                    html += `</div>`;
                    return html;
                }
            }
        ]
    });
    // Return the DataTable object
    return dataTable;
}

// This Function for Delete Container Type Document...
function deleteContainerType(url) {
    debugger;

    // Show a confirmation dialog using SweetAlert
    swal({
        title: 'Do you want to delete This Container Type MetaData?',

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

// This function for show UserGrops list in dropdown of Document popup view
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

// Function to get the current date and time in the format yyyy-mm-dd HH:mm:ss
function getCurrentDateTime() {
    debugger;
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

// Function to set the value of the input field and checkbox on first load
function setInitialValues() {
    debugger;
    if (firstLoad) {
        var metadataList = document.querySelector('.Meta-Data');
        var newRow = document.createElement('tr');
        newRow.setAttribute('data-metadata-name', getCurrentDateTime());
        newRow.innerHTML = `
        <td class="col-8">
         
             <input type="text" class="form-control no-border"  readonly>
        </td>
        <td class="col-2 text-center align-middle">
            <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
                <input type="checkbox" class="form-check-input" unchecked>
            </label>
        </td>
        <td class="col-1 text-center align-middle">
            <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
            data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'add')">
                <i class="fa fa-trash"></i>
            </a>
        </td>
        <td class="col-1 text-center align-middle">
            <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top"
                data-bs-original-title="Add MetaData" data-bs-toggle="modal" data-bs-target="#metadata-modal">
               <i class="fa fa-plus-circle plus-icon"></i>
            </a>
        </td>
        `;
        metadataList.appendChild(newRow);

        firstLoad = false;
    }
}

// This function for show all table data into MetaData popUp view
function fetchMetadata(tableBodyId, selectedMetaData) {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/Document/GetAllDocTypeMetadata',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            if (data.success === true) {
                var metaData = data.metaData;
                var tableBody = document.getElementById(tableBodyId);
                tableBody.innerHTML = ''; // Clear existing rows before populating

                metaData.forEach(function (metadata) {
                    var row = document.createElement('tr');
                    var nameCell = document.createElement('td');
                    var actionsCell = document.createElement('td');

                    nameCell.textContent = metadata.metadataName;
                    actionsCell.innerHTML = `
                        <div class="form-check text-center">
                        <input type="checkbox" class="form-check-input" data-metadata="${metadata.metadataName}"${selectedMetaData.
                            some(item => item.metadataName === metadata.metadataName && item.isChecked) ? 'checked' : ''}>
                        </div>
                         `;

                    row.appendChild(nameCell);
                    row.appendChild(actionsCell);
                    tableBody.appendChild(row);
                });
            }
        },
        error: function (error) {
            console.error(error.statusText);
        }
    });
}


// This function for Add MetaData List to the Container Type PopUp View
function addMetadata(context) {
    debugger;
    var metadataListSelector = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataList = document.querySelector(metadataListSelector);
    var metaDataTableBodyId = (context === 'add') ? 'metaDataBody' : 'updateMetaDataTableBody';
    var metaDataModal = (context === 'add') ? 'metadata-modal' : 'updateMetadata-modal';

    // Get the selected metadata from the modal's checkboxes
    var selectedMetadata = [];
    var checkboxes = document.querySelectorAll(`#${metaDataTableBodyId} input[type="checkbox"]`);

    var addMetaData = [];
    if (context === 'add') {
        $(".Meta-Data tr").each(function () {
            var metadataName = $(this).attr("data-metadata-name");
            var isChecked = $(this).find("input[type='checkbox']").prop("checked");
            addMetaData.push({ metadataName: metadataName, isChecked: isChecked });
        });
    } else {
        var rows = document.querySelectorAll('.Update-Meta-Data tr');
        rows.forEach(function (row) {
            debugger;
            var metadataName = row.querySelector('.col-8').textContent;
            var isChecked = row.querySelector("input[type='checkbox']").checked;
            addMetaData.push({ metadataName: metadataName, isChecked: isChecked });
        });
    }
    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            var metadataName = checkbox.dataset.metadata;
            selectedMetadata.push(metadataName);
        }
    });

    // Clear existing rows before populating
    metadataList.innerHTML = '';

    // Function to show the custom alert modal
    function showAlert(message) {
        // Set the alert message
        $("#customAlertMessage").text(message);

        // Show the custom alert modal
        $("#customAlertModal").modal('show');
    }

    // Check the limit and add metadata rows
    if (selectedMetadata.length < 6) {
        selectedMetadata.forEach(function (metadataName) {
            debugger;
            addMetadataRow(metadataName, context, addMetaData);
            addedMetadata.push(metadataName); // Add to the addedMetadata array
            //numRowsAdded++;
        });
    } else {
        debugger;
        // Display a message in the custom alert modal when the maximum limit is reached
        showAlert("Maximum limit of 5 metadata rows has been reached.");
        return;
    }


    // Update Case
    if (context === 'update') {
        // Close the update-MetaData-modal after adding metadata
        var metadataModal = document.getElementById(metaDataModal);
        var bootstrapMetadataModal = bootstrap.Modal.getInstance(metadataModal);
        bootstrapMetadataModal.hide();

        // When the "Update-MetaData-modal" is hidden, show the "Update File Type Modal" modal
        $('#updateMetadata-modal').on('hidden.bs.modal', function (event) {
            debugger;
            showUpdateContainerModal();
        });
    }

    // Add Case
    if (context === 'add') {
        // Close the metadata-modal after adding metadata
        var metadataModal = document.getElementById(metaDataModal);
        var bootstrapMetadataModal = bootstrap.Modal.getInstance(metadataModal);
        bootstrapMetadataModal.hide();
        // When the "Metadata Modal" is hidden, show the "Add New Document" modal
        $('#metadata-modal').on('hidden.bs.modal', function (event) {
            debugger;
            showAddContainerModal();
        });
    }
}

// This function for Populate MetaData List to the Container Type PopUp View
function addMetadataRow(metadataName, context, addMetaData) {
    debugger;
    var metaDataContext = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataListTable = document.querySelector(metaDataContext);

    var newRow = document.createElement('tr');
    if (context == 'add') {
        newRow.setAttribute('data-metadata-name', metadataName);
        newRow.innerHTML = `

                    <td class="col-8">${metadataName}</td>
                    <td class="col-2 text-center align-middle">
                     <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
                        <input type="checkbox" class="form-check-input" ${isCheckedInAddMetaData(metadataName.isChecked, addMetaData) ? 'checked' : ''}>
                     </label>
                    </td>


                    <td class="col-1 text-center align-middle">
                        <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
                           data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'add')">
                            <i class="fa fa-trash"></i>
                        </a>
                    </td>
                    <td class="col-1 text-center align-middle">
                        <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top" data-bs-original-title="Add MetaData"
                            data-bs-toggle="modal" data-bs-target="#metadata-modal">
                            <i class="fa fa-plus-circle plus-icon"></i>
                        </a>
                    </td>
                `;
    } else {
        newRow.setAttribute('update-metadata-name', metadataName);
        newRow.innerHTML = `
        <td class="col-8">${metadataName}</td>
        <td class="col-2 text-center align-middle">
            <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
              <input type="checkbox" class="form-check-input" ${isCheckedInAddMetaData(metadataName, addMetaData) ? 'checked' : ''}>
            </label>
        </td>
        <td class="col-1 text-center align-middle">
            <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
            data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'update')">
                <i class="fa fa-trash"></i>
            </a>
        </td>
        <td class="col-1 text-center align-middle">
            <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top" data-bs-original-title="Add MetaData"
                data-bs-toggle="modal" data-bs-target="#updateMetadata-modal">
                <i class="fa fa-plus-circle plus-icon"></i>
            </a>
        </td>
         `;
    }

    metadataListTable.appendChild(newRow);
}

function isCheckedInAddMetaData(metadataName, addMetaData) {
    var foundMetadata = addMetaData.find(function (item) {
        return item.metadataName === metadataName;
    });

    return foundMetadata && foundMetadata.isChecked;
}

// This function for Iport MetaData From CSV/Excel file....
function submitForm() {
    debugger;
    var fileInput = document.getElementById("metaDataFile");
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append("formFile", file);

    $.ajax({
        url: "/CompanyAdmin/Containertype/ImportMetaData",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            debugger;
            if (response.status) {
                debugger;
                // Hide the "Import MetaData" modal
                $('#importMetaDataModal').modal('hide');

                // Show the "Select Metadata" modal with updated data
                showMetadataModal();
            }
        }
    });
}

function showMetadataModal() {
    debugger;
    // Clear existing data in the table body
    $('#metadataTableBody').empty();

    fetchMetadata();

    $('#addContainerTypeModal').modal('hide');
    // Once you have loaded the data, show the modal
    $('#metadata-modal').modal('show');
}



//function addValueButton() {
//    debugger;
//    // Remove any existing modal
//    $('#valueModal').remove();

//    // Create a Bootstrap modal dynamically
//    var modalContent = '<div class="modal fade" id="valueModal" tabindex="-1" role="dialog" aria-labelledby="valueModalLabel" aria-hidden="true">' +
//        '<div class="modal-dialog" role="document">' +
//        '<div class="modal-content">' +
//        '<div class="modal-header">' +
//        '<h5 class="modal-title" id="valueModalLabel">Enter the name of the new value:</h5>' +
//        '<button type="button" class="close" data-bs-dismiss="modal" aria-label="Close">' +
//        '<span aria-hidden="true">&times;</span>' +
//        '</button>' +
//        '</div>' +
//        '<div class="modal-body">' +
//        '<input type="text" id="valueInput" class="form-control" placeholder="Enter value name">' +
//        '</div>' +
//        '<div class="modal-footer">' +
//        '<button type="button" class="btn btn-primary" onclick="saveValue()">Save</button>' +
//        '<button type="button" class="btn btn-secondary" data-bs-dismiss="modal" onclick="closeModal()">Cancel</button>' +
//        '</div>' +
//        '</div>' +
//        '</div>' +
//        '</div>';

//    // Append modal to the body
//    $('body').append(modalContent);

//    // Show the modal
//    $('#valueModal').modal('show');

//    // Attach an event listener to remove the modal from the DOM when it's hidden
//    $('#valueModal').on('hidden.bs.modal', function () {
//        $(this).remove();
//    });
//}

//function saveValue() {
//    debugger;
//    // Get the value from the input field
//    var name = $('#valueInput').val();

//    if (name) {
//        var $table = $('#tblValues');
//        var $tbody = $table.find('tbody');

//        // Calculate the next value-id based on the current maximum
//        var valueIds = $tbody.find('.value-id').map(function () {
//            return parseInt($(this).text()) || 0;
//        }).get();

//        var maxValueId = valueIds.length === 0 ? 100 : Math.max.apply(null, valueIds);

//        var newId = maxValueId + 1;

//        // Create a new row for the table with the dynamically generated value-id
//        var newRow = '<tr>' +
//            '<td><input type="checkbox" class="value-checkbox" value="' + newId + '"></td>' +
//            '<td class="value-id">' + newId + '</td>' +
//            '<td class="value-name">' + name + '</td>' +
//            '</tr>';

//        // Append the new row to the table
//        $tbody.append(newRow);

//        // Hide and remove the modal from the DOM
//        $('#valueModal').modal('hide').remove();
//    }
//}


function addValueButton() {
    // Create a Bootstrap modal
    var modalContent = '<div class="modal fade" id="valueModal" tabindex="-1" role="dialog" aria-labelledby="valueModalLabel" aria-hidden="true">' +
        '<div class="modal-dialog" role="document">' +
        '<div class="modal-content">' +
        '<div class="modal-header">' +
        '<h5 class="modal-title" id="valueModalLabel">Enter the name of the new value:</h5>' +
        '<button type="button" class="close" data-dismiss="modal" aria-label="Close" onclick="closeModal()">' +
        '<span aria-hidden="true">&times;</span>' +
        '</button>' +
        '</div>' +
        '<div class="modal-body">' +
        '<input type="text" id="valueInput" class="form-control" placeholder="Enter value name">' +
        '</div>' +
        '<div class="modal-footer">' +
        '<button type="button" class="btn btn-primary" onclick="saveValue()">Save</button>' +
        '<button type="button" class="btn btn-secondary" data-dismiss="modal" onclick="closeModal()">Cancel</button>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>';

    // Append modal to the body
    $('body').append(modalContent);

    // Show the modal
    $('#valueModal').modal('show');
}

function saveValue() {
    // Get the value from the input field
    var name = $('#valueInput').val();

    if (name) {
        var $table = $('#tblValues');
        var $tbody = $table.find('tbody');

        // Calculate the next value-id based on the current maximum
        var valueIds = $tbody.find('.value-id').map(function () {
            return parseInt($(this).text()) || 0;
        }).get();

        var maxValueId = valueIds.length === 0 ? 100 : Math.max.apply(null, valueIds);

        var newId = maxValueId + 1;

        // Create a new row for the table with the dynamically generated value-id
        var newRow = '<tr>' +
            '<td><input type="checkbox" class="value-checkbox" value="' + newId + '"></td>' +
            '<td class="value-id">' + newId + '</td>' +
            '<td class="value-name">' + name + '</td>' +
            '</tr>';

        // Append the new row to the table
        $tbody.append(newRow);

        // Close the modal
        $('#valueModal').modal('hide');
    }
}

function closeModal() {
    // Close the modal without saving
    $('#valueModal').modal('hide');
}


function showAlert(message) {
    // Set the alert message
    $("#customAlertMessage").text(message);

    // Show the custom alert modal
    $("#customAlertModal").modal('show');
}

$("#deleteValueButton1").on("click", function () {
    var $checkedCheckboxes = $("#tblValues1 tbody input[type='checkbox']:checked");

    if ($checkedCheckboxes.length === 0) {
        showAlert("Please select at least one row to delete.");
        return;
    }

    // Show the delete confirmation modal
    $("#deleteConfirmationModal").modal('show');

    // Handle the delete action when the user confirms
    $("#confirmDelete").on("click", function () {
        var deletedValueIds = [];

        $checkedCheckboxes.each(function () {
            var $row = $(this).closest("tr");
            var valueId = $row.find('.value-id').text();
            var name = $row.find('.value-name').text();
            deletedValueIds.push({ valueId: valueId, name: name });
            $row.remove();
        });

        $.ajax({
            url: '/CompanyAdmin/ContainerType/DeleteValues',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(deletedValueIds),
            success: function (response) {
                console.log(response);
            },
            error: function (error) {
                console.error(error);
                showAlert('Error deleting values. Please try again.');
            }
        });

        // Hide the modal after deletion
        $("#deleteConfirmationModal").modal('hide');
    });
});




//$("#deleteValueButton1").on("click", function () {
//    debugger;
//    // Get all checkboxes within the table body that are checked
//    var $checkedCheckboxes = $("#tblValues1 tbody input[type='checkbox']:checked");

//    // Check if any checkbox is checked
//    if ($checkedCheckboxes.length === 0) {
//        alert("Please select at least one row to delete.");
//        return;
//    }

//    // Confirm with the user before deleting
//    if (confirm("Do you want to delete the selected rows?")) {
//        // Iterate through each checked checkbox
//        $checkedCheckboxes.each(function () {
//            // Remove the corresponding row from the table
//            $(this).closest("tr").remove();
//        });

//        // Optionally, you can perform additional actions after deleting rows
//        // For example, you might want to update the server or perform other tasks.
//    }
//});


// This function for Saving Document data into database "POST" Method
$(document).ready(function () {
    $("#containerName").on("input", function () {
        $("#containerNameError").empty();
    });

    $("#groupList").on("input", function () {
        $("#UserGroupError").empty();
    });
});

function validateForm() {
    // Clear existing error messages
    $(".error-message").empty();

    // Validation for Container Type Name
    var containerTypeName = $("#containerName").val();
    if (!containerTypeName) {
        $("#containerNameError").text("Container Type Name is required.");
        return false;
    }

    // Validation for User Group
    var selectedGroupsCount = $("#groupList option:selected:not([disabled])").length;
    if (selectedGroupsCount === 0) {
        $("#UserGroupError").text("At least one User Group must be selected.");
        return false;
    }

    return true;
}

function getFormData() {
    var form = $("#addContainerTypeForm");
    var formData = new FormData(form[0]);

    var selectedUserGroups = $("#groupList option:selected").map(function () {
        return $(this).val();
    }).get();

    var selectedMetadata = $(".Meta-Data tr").map(function () {
        var metadataName = $(this).attr("data-metadata-name");
        var isChecked = $(this).find("input[type='checkbox']").prop("checked");
        return { metadataName: metadataName, isRequired: isChecked };
    }).get();

    formData.append("SelectedUserGroups", JSON.stringify(selectedUserGroups));
    formData.append("SelectedMetadata", JSON.stringify(selectedMetadata));

    var selectedValues = $('#tblValues tbody tr').map(function () {
        var valueId = $(this).find('.value-id').text();
        var name = $(this).find('.value-name').text();
        return { valueId: valueId, name: name };
    }).get();

    formData.append("SelectedValues", JSON.stringify(selectedValues));

    return formData;
}

function saveContainerType() {
    if (!validateForm()) {
        return; // Stop the process if validation fails
    }

    var formData = getFormData();

    $.ajax({
        url: $("#addContainerTypeForm").attr("action"),
        type: "POST",
        data: formData,
        dataType: "json",
        contentType: false, // Important for sending FormData
        processData: false, // Important for sending FormData
        success: function (response) {
            if (response.success) {
                resetContainerTypeModal();
                $("#addContainerTypeModal").modal("hide");
                not1(response.message);
                dataTable.ajax.reload();
            } else {
                if (response.isExistContainerNameError) {
                    $("#containerNameError").text(response.errorMessage);
                } else {
                    swal({
                        title: 'Error',
                        text: 'An error occurred: ' + response.errorMessage,
                        icon: 'error'
                    });
                }
            }
        },
        error: function (xhr, status, error) {
            var errorMessage = "An error occurred: " + (error ? error : "Undefined error");
            swal({
                title: 'Error',
                text: errorMessage,
                icon: 'error'
            });
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

// Function to reset the form fields inside the #addDocumentModal
function resetContainerTypeModal() {
    debugger;
    $('#containerName').val(''); // Reset the text input field
    $('#containerDataType').val([]);
    $('#groupList').val([]); // Reset the multi-select dropdown
    $('.Meta-Data').empty(); // Clear the metadata table

    // Reset the firstLoad variable to true
    firstLoad = true;

    setInitialValues();
    GenerateUniqueId();
}

function GenerateUniqueId() {
    $.ajax({
        url: "/CompanyAdmin/ContainerType/GenerateUniqueId", // Replace with your API endpoint URL
        method: "GET",
        success: function (data) {
            debugger;
            $("#id").val(data); // Populate the generated ID in the input field
        },
        error: function (error) {
            console.error("Error generating unique ID:", error);
        }
    });
}





var selectedCheckboxes = [];
function editContainerType(url) {
    debugger;
    var modal = $('#updateContainerTypeModal');

    // Clear the existing metadata rows
    clearMetadataListTable();

    // Clear the existing values in the table
    $('#tblValues1 tbody').empty();

    // Perform the AJAX request
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            debugger;
            // Handle the response from the server
            if (response.success) {
                console.log(response);
                debugger;
                var fileTypeDoc = response.data;
                $('#id_1').val(fileTypeDoc.id);
                $('#containerName_1').val(fileTypeDoc.containerName);
                // Populate the filename input field
                $('#containerDataType_1').val(fileTypeDoc.dataType);
                // Assuming you have retrieved the user group names array from the response
                var selectedUserGroupNames = fileTypeDoc.userGroups;
                // Fetch and populate the dropdown options for user groups
                fetchDropdownOptions('#userGroupNames', selectedUserGroupNames);
                // Populate the metadata list
                var metadataList = fileTypeDoc.metaDataList;
                metadataList.forEach(function (metaData) {
                    debugger;
                    // Call a function to add metadata to the list
                    updateMetaDataRow(metaData);
                });

                //// Access and display the selectedValues
                var selectedValues = fileTypeDoc.selectedValues;

                if (selectedValues && selectedValues.length > 0) {
                    // Loop through selectedValues and add rows to the table with checkboxes
                    for (var i = 0; i < selectedValues.length; i++) {
                        var value = selectedValues[i];
                        // Example: Add a row to the table with a checkbox, valueId, and valueName
                        $('#tblValues1 tbody').append(
                            '<tr>' +
                            '<td><input type="checkbox" class="value-checkbox" value="' + value.valueId + '"></td>' +
                            '<td class="wd-15p border-bottom-0">' + value.valueId + '</td>' +
                            '<td class="wd-15p border-bottom-0">' + value.name + '</td>' +
                            '</tr>'
                        );
                    }
                }


                // Show the modal
                modal.modal('show');
            } else {
                toastr.error(response.message);
            }
        },
        error: function (error) {
            debugger;
            alert(error);
            // Handle AJAX error
            // ...
        }
    });


    function closeModal1() {
        // Additional actions you want to perform before closing the modal
        // ...

        // Close the modal by hiding it
        $('#EditModal').modal('hide');
    }

    var globalEditedValues = []; // Define globalEditedValues in the global scope

    $(document).on('click', '#editValueButton', function () {
        debugger;
        // Initialize editedValues array for each click
        var editedValues = [];

        // Clear existing modals
        $('.customEditModal').remove();

        // Iterate over all checkboxes to capture their values
        $('#tblValues1 tbody input.value-checkbox:checked').each(function () {
            debugger;
            var row = $(this).closest('tr');
            var valueId = row.find('.wd-15p.border-bottom-0:eq(0)').text();

            // Creating a custom modal HTML string
            var modalHtml = `
            <div class="modal customEditModal" tabindex="-1" role="dialog" id ="EditModal">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Edit Name</h5>
                           <button type="button" class="close btn-red" data-bs-dismiss="modal" aria-label="Close" fdprocessedid="pbt1wj">
                    <span aria-hidden="true">&times;</span>
                </button>
                        </div>
                        <div class="modal-body">
                            <label for="editName">Enter an edit name:</label>
                            <input type="text" id="editName" class="form-control" value="${row.find('.wd-15p.border-bottom-0:eq(1)').text()}">
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-primary saveChanges">Save changes</button>
                            <button type="button" class="btn btn-success" data-bs-dismiss="modal">Back</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

            // Append the custom modal HTML to the body
            $('body').append(modalHtml);

            // Show the custom modal
            $('.customEditModal').modal('show');

            // Handle the Save Changes button click
            $('.customEditModal .saveChanges').off('click').on('click', function () {
                debugger;
                var editName = $('#editName').val();
                if (editName.trim() !== "") {
                    row.find('.wd-15p.border-bottom-0:eq(1)').text(editName);
                    editedValues.push({ valueId: valueId, name: editName });
                }

                // Close the custom modal
                $('.customEditModal').modal('hide');

                // Remove the custom modal HTML from the body
                $('.customEditModal').remove();

                // Close the custom modal
                closeModal1();

                // Append editedValues to the globalEditedValues array
                globalEditedValues = globalEditedValues.concat(editedValues);


                // Call a function to store edited values
                updateEditedValues(globalEditedValues);
            });
        });
    });

    function updateEditedValues(values) {
        debugger;

        // This function stores the edited values for use in the updateContainer function
        globalEditedValues = values;
        console.log(globalEditedValues); // Log to check if values are correct
    }

    function addValueButton() {
        // Get the last updated ID from the table
        var lastUpdatedId = getLastUpdatedId();

        // Increment the last updated ID for the new row
        var newId = lastUpdatedId + 1;

        // Create a new row with the updated ID
        var newRow = createNewRow(newId);

        // Append the new row to the table
        var tableBody = document.getElementById("valuesTableBody");
        tableBody.appendChild(newRow);
    }

    function getLastUpdatedId() {
        // Get all rows in the table
        var rows = document.querySelectorAll("#tblValues1 tbody tr");

        // Check if there are any rows
        if (rows.length > 0) {
            // Get the last row
            var lastRow = rows[rows.length - 1];

            // Get the ID from the last row
            var lastUpdatedId = parseInt(lastRow.querySelector("td:nth-child(2)").textContent);

            return isNaN(lastUpdatedId) ? 0 : lastUpdatedId;
        } else {
            return 0;
        }
    }

    function createNewRow(newId) {
        // Create a new row
        var newRow = document.createElement("tr");

        // Add cells to the new row
        var cell1 = document.createElement("td");
        var cell2 = document.createElement("td");
        var cell3 = document.createElement("td");

        // Set the content of the cells (you can customize this based on your needs)
        cell1.textContent = "";
        cell2.textContent = newId;
        cell3.textContent = "New Value"; // Default value for the new row

        // Append cells to the new row
        newRow.appendChild(cell1);
        newRow.appendChild(cell2);
        newRow.appendChild(cell3);

        return newRow;
    }


    //var globalEditedValues = []; // Define globalEditedValues in the global scope

    ////$(document).on('click', '#editValueButton', function () {
    ////    debugger;
    ////    updateValue();
    ////});

    //var selectedCheckboxes = [];

    //$(document).on('click', '#editValueButton', function () {
    //    debugger;
    //    // Initialize editedValues array for each click
    //    var editedValues = [];

    //    selectedCheckboxes = $('#tblValues1 tbody').find('input.value-checkbox:checked');

    //    if (selectedCheckboxes.length === 0) {
    //        alert("Please select at least one checkbox.");
    //        return;
    //    }

    //    // Iterate over all checkboxes to capture their values
    //    $('#tblValues1 tbody input.value-checkbox').each(function () {
    //        debugger;
    //        var row = $(this).closest('tr');
    //        var valueId = row.find('.wd-15p.border-bottom-0:eq(0)').text();
    //        var editName = prompt("Enter an edit name:", row.find('.wd-15p.border-bottom-0:eq(1)').text());

    //        if (editName !== null) {
    //            row.find('.wd-15p.border-bottom-0:eq(1)').text(editName);
    //            editedValues.push({ valueId: valueId, name: editName });
    //        }
    //    });

    //    debugger;
    //    // Append editedValues to the globalEditedValues array
    //    globalEditedValues = globalEditedValues.concat(editedValues);

    //    // Call a function to store edited values
    //    updateEditedValues(globalEditedValues);
    //});

    //function updateEditedValues(values) {
    //    debugger;
    //    // This function stores the edited values for use in the updateContainer function
    //    globalEditedValues = values;
    //}


    $(document).on('click', '#updateContainerTypeBtn', function () {
        debugger;
        // updateContainer();
        var form = $("#updateContainerTypeForm");

        // Clear existing error messages
        $(".error-message").empty();
        var isValid = true;

        // Validation for Container Type Name
        var containerTypeName = $("#containerName_1").val();
        if (!containerTypeName) {
            $("#containerNameError_1").text("Container Type Name is required.");
            isValid = false;
        }

        // Clear the Container Type Name error message when input changes
        $("#containerName_1").on("input", function () {
            $("#containerNameError_1").empty();
        });

        // Get selected user group list from multi-select dropdown
        var selectedUserGroups = [];
        $("#userGroupNames option:selected").each(function () {
            selectedUserGroups.push($(this).val());
        });

        // Get selected metadata list from the metadata table
        var selectedMetadata = [];

        // Iterate through each row in the metadata list
        var rows = document.querySelectorAll('.Update-Meta-Data tr');
        rows.forEach(function (row) {
            var metadataName = row.querySelector('.col-8').textContent;
            var isChecked = row.querySelector("input[type='checkbox']").checked;
            selectedMetadata.push({ metadataName: metadataName, isRequired: isChecked });
        });

        // Retrieve the edited values from the stored global variable
        var editedValues = globalEditedValues;

        // Add the selected values to the FormData
        var formData = new FormData(form[0]);
        formData.append("UpdatedUserGroups", JSON.stringify(selectedUserGroups));
        formData.append("UpdatedMetadata", JSON.stringify(selectedMetadata));
        formData.append("UpdatedValues", JSON.stringify(editedValues));

        // Perform the AJAX request with the updated form data
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
                    // numRow sAdded = 0;
                    var metadataListTable = document.querySelector('.Update-Meta-Data');
                    metadataListTable.innerHTML = '';
                    // Hide the modal
                    $("#updateContainerTypeModal").modal("hide");
                    not1(response.message);
                    dataTable.ajax.reload();

                    // Reload the page after a successful update
                    window.location.reload();
                } else {
                    if (response.isExistContainerNameError) {
                        // Display the error message on your view
                        $("#containerNameError_1").text(response.message);
                    } else {
                        // Use SweetAlert for other error scenarios
                        swal({
                            title: 'Error',
                            text: 'An error occurred: ' + response.message,
                            icon: 'error'
                        });
                    }
                }
            },
            error: function (error) {
                // Use SweetAlert for other error scenarios
                swal({
                    title: 'Error',
                    text: 'An error occurred: ' + error.message,
                    icon: 'error'
                });
            }
        });
    });

    // Now globalEditedValues will contain all the values, including updated and unedited values

    //  showvalueModal();



    //function showvalueModal() {
    //    // Hide the editviewAllValueModal if it's shown
    //    var editviewAllValueModal = document.getElementById('editviewAllValueModal');
    //    var bootstrapEditviewAllValueModal = bootstrap.Modal.getInstance(editviewAllValueModal);
    //    bootstrapEditviewAllValueModal.hide();

    //    // Show the updateContainerTypeModal
    //    var updateContainerTypeModal = document.getElementById('updateContainerTypeModal');
    //    var bootstrapUpdateContainerTypeModal = bootstrap.Modal.getInstance(updateContainerTypeModal);
    //    bootstrapUpdateContainerTypeModal.show();

    //}
    //$(document).ready(function () {
    //    $('#editValueButton').on('click', function () {
    //        debugger;
    //        // Initialize an array to store selected checkbox data
    //        var selectedData = [];

    //        // Iterate through the selected checkboxes
    //        $('#tblValues1 tbody').find('input.value-checkbox:checked').each(function () {
    //            var row = $(this).closest('tr');
    //            var valueId = row.find('.wd-15p.border-bottom-0:eq(1)').text();
    //            var name = row.find('.wd-15p.border-bottom-0:eq(2)').text();
    //            selectedData.push({ valueId: valueId, name: name });
    //        });

    //        // Check if any checkboxes are selected
    //        if (selectedData.length > 0) {
    //            var editName = prompt("Enter an edit name:");

    //            if (editName !== null) {
    //                // Update the names of the selected checkboxes with the edit name
    //                selectedData.forEach(function (data) {
    //                    data.name = editName;
    //                });

    //                // Update the table with the edited names
    //                $('#tblValues1 tbody').find('input.value-checkbox:checked').each(function (index) {
    //                    var row = $(this).closest('tr');
    //                    row.find('.wd-15p.border-bottom-0:eq(2)').text(selectedData[index].name);
    //                });
    //            } else {
    //                alert("Please enter a name.");
    //            }
    //        } else {
    //            alert("Please select at least one checkbox.");
    //        }
    //    });
    //});



    function clearMetadataListTable() {
        var metadataListTable = document.querySelector('.Update-Meta-Data');
        metadataListTable.innerHTML = '';
    }

    // This function for Update Container MetaData PopUp View Showing Metadata list....
    function updateMetaDataRow(metaData) {
        debugger;
        var metadataListTable = document.querySelector('.Update-Meta-Data');

        if (metadataListTable.children.length === 0) {
            // Clear existing rows before populating
            metadataListTable.innerHTML = '';
        }

        var newRow = document.createElement('tr');
        newRow.innerHTML = `
        <td class="col-8">${metaData.metaDataName}</td>
        <td class="col-2 text-center align-middle">
            <label class="form-check-label d-flex justify-content-center align-items-center" style="height: 100%;">
                <input type="checkbox" class="form-check-input" ${metaData.required ? 'checked' : ''}>
            </label>
        </td>
        <td class="col-1 text-center align-middle">
            <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
                data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'update')">
                <i class="fa fa-trash"></i>
            </a>
        </td>
        <td class="col-1 text-center align-middle">
            <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add Metadata" data-bs-placement="top"
                data-bs-original-title="Add MetaData" data-bs-toggle="modal" data-bs-target="#updateMetadata-modal">
                <i class="fa fa-plus-circle plus-icon"></i>
            </a>
        </td>
         `;

        metadataListTable.appendChild(newRow);
    }


    // This function for remove metadata from list
    function removeMetadata(button, context) {
        debugger;
        var row = button.closest('tr');
        var metadataListSelector = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
        var metadataList = document.querySelector(metadataListSelector);

        if (metadataList.children.length > 1) {
            var metadataName = row.querySelector('.col-8').textContent.trim();
            var indexToRemove = addedMetadata.indexOf(metadataName);
            if (indexToRemove !== -1) {
                addedMetadata.splice(indexToRemove, 1);
            }
            row.remove();
            // Update the generated file name
            updateGeneratedFileName(context);
        } else {
            showAlert("Cannot remove the only metadata row 1.");
        }
    }



    // Example usage
    // removeMetadata(myButtonElement, 'add');


    // Example usage
    // removeMetadata(myButtonElement, 'add');






    // Download PDF File
    function exportPdf() {
        debugger;
        window.location.href = '/CompanyAdmin/ContainerType/ExportPdf';
    }

    // Download All Container MetaData in Excel Format...
    function exportExcel() {
        window.location.href = '/CompanyAdmin/ContainerType/ExportExcel';
    }
}