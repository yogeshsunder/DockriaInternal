var dataTable = null;
var numRowsAdded = 0; // Keep track of the number of rows added
// Define a global variable to keep track of added metadata
var addedMetadata = [];
// Define a global variable to keep track of the current index
var rowIndex = 0;
var defaultRowRemoved = false;
// Check if the page is loaded for the first time
let firstLoad = true;

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
    // this function for load all details into Index view of FileName Type....
    loadDataTable();

    // this function for show Metadata PopUp view and Hide Add Document popUp view
    $('#metadata-modal').on('show.bs.modal', function (event) {
        debugger;
        var addDocumentModal = document.getElementById('addFileNameTypeModal');
        var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
        bootstrapAddDocumentModal.hide();
        debugger;
        // Create an array to hold metadata names and separators
        var metadataNamesList = [];

        // Iterate through each row in the metadata list
        var rows = document.querySelectorAll('.Meta-Data tr');
        rows.forEach(function (row) {
            var selectedMetadataNames = row.querySelector('.col-8').textContent;
            var isChecked = true;

            // Push the metadata names and separator as an object into the array
            metadataNamesList.push({
                metadataName: selectedMetadataNames,
                isChecked: isChecked
            });
        });
        fetchMetadata('metadataTableBody', metadataNamesList);
    });

    // this function for show UPDATE Case Metadata PopUp view and Hide Update Document popUp view
    $('#updateMetadata-modal').on('show.bs.modal', function (event) {
        debugger;
        var updateDocumentModal = document.getElementById('editFileNameTypeModal');
        var bootstrapUpdateDocumentModal = bootstrap.Modal.getInstance(updateDocumentModal);
        bootstrapUpdateDocumentModal.hide();

        var metadataNamesList = [];

        // Iterate through each row in the metadata list
        var rows = document.querySelectorAll('.Update-Meta-Data tr');
        rows.forEach(function (row) {
            var selectedMetadataNames = row.querySelector('.col-8').textContent;
            var isChecked = true;

            // Push the metadata names and separator as an object into the array
            metadataNamesList.push({
                metadataName: selectedMetadataNames,
                isChecked: isChecked
            });
        });
        fetchMetadata('updateMetaDataTableBody', metadataNamesList);
    });

    // Add event listener to the "SAVE" button
    $(document).on('click', '#saveFileTypeButton', function () {
        debugger;
        saveDocument();
    });

    // Add event listener to the "UPDATE" button
    $(document).on('click', '#updateFileTypeButton', function () {
        debugger;
        updateDocument();
    });

    $('#metadata-modal').on('hidden.bs.modal', function () {
        showAddDocumentModal();
    });

    $('#updateMetadata-modal').on('hidden.bs.modal', function () {
        showUpdateFileTypeModal();
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

    $('#addFileNameTypeModal').on('hidden.bs.modal', function () {
        debugger;
        $('#fileName').val(''); // Reset the text input field
        // Remove all metadata rows, including the default row
        $('.Meta-Data tr').remove();
        firstLoad = true;
        setInitialValues();
    });
});


$('#metadata-modal').on('hidden.bs.modal', function (event) {
    debugger;

    // Reset the flag when metadata modal is closed
    defaultRowRemoved = false;
    // Show the addDocumentTypeModal only if metadata modal is closed
    if (!defaultRowRemoved) {
        showAddDocumentModal();
    }
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


// This Function for search fuctionality into metadata list....
function filterMetadata(context, searchValue) {
    debugger;
    var metaDataTableId = (context === 'add') ? '#metadataTableBody' : '#updateMetaDataTableBody';
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

function showAddDocumentModal() {
    debugger;
    var addDocumentModal = document.getElementById('addFileNameTypeModal');
    var bootstrapAddDocumentModal = bootstrap.Modal.getInstance(addDocumentModal);
    bootstrapAddDocumentModal.show();
}

function showUpdateFileTypeModal() {
    debugger;
    var updateFileNameTypeModal = document.getElementById('editFileNameTypeModal');
    var bootStrapEditModal = bootstrap.Modal.getInstance(updateFileNameTypeModal);
    bootStrapEditModal.show();
}

// This function for show all table data into MetaData popUp view
//function fetchMetadata(tableBodyId, selectedMetaData) {
//    debugger;
//    $.ajax({
//        url: '/CompanyAdmin/Document/GetAllDocTypeMetadata',
//        type: 'GET',
//        dataType: 'json',
//        success: function (data) {
//            debugger;
//            if (data.success === true) {
//                var fileTypeMetaData = data
//                    .data;
//                var tableBody = document.getElementById(tableBodyId);
//                tableBody.innerHTML = ''; // Clear existing rows before populating

//                fileTypeMetaData.forEach(function (metadata) {
//                    var row = document.createElement('tr');
//                    var nameCell = document.createElement('td');
//                    var actionsCell = document.createElement('td');

//                    nameCell.textContent = metadata.metadataName;
//                    actionsCell.innerHTML = `
//                        <div class="form-check text-center">
//                        <input type="checkbox" class="form-check-input" data-metadata="${metadata.metadataName}"${selectedMetaData.
//                            some(item => item.metadataName === metadata.metadataName && item.isChecked) ? 'checked' : ''}>
//                        </div>
//                         `;

//                    row.appendChild(nameCell);
//                    row.appendChild(actionsCell);
//                    tableBody.appendChild(row);
//                });
//            }
//        },
//        error: function (error) {
//            console.error(error.statusText);
//        }
//    });
//}

function fetchMetadata(tableBodyId, selectedMetadata) {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/Document/GetAllDocTypeMetadata',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            if (data.success === true) {
                var metaData = data.metaData; // Separate metadata array
                var containerData = data.containerData; // Separate container data array

                var tableBody = document.getElementById(tableBodyId);
                tableBody.innerHTML = ''; // Clear existing rows before populating

                // Display metadata items
                metaData.forEach(function (metadata) {
                    var row = document.createElement('tr');
                    var metadataCell = document.createElement('td'); // Create a cell for metadataName
                    var metadataCheckboxCell = document.createElement('td'); // Create a cell for the metadata checkbox

                    metadataCell.textContent = metadata.metadataName; // Populate metadataName

                    row.appendChild(metadataCell); // Add metadataCell to the row

                    // Create a checkbox for metadata
                    var isMetadata = true;
                    var metadataCheckboxDiv = createCheckboxDiv(metadata.metadataName, selectedMetadata, isMetadata);
                    metadataCheckboxCell.appendChild(metadataCheckboxDiv);

                    row.appendChild(metadataCheckboxCell); // Add metadataCheckboxCell to the row
                    tableBody.appendChild(row);
                });

                // Display container names below metadata list
                containerData.forEach(function (container) {
                    var row = document.createElement('tr');
                    var containerCell = document.createElement('td'); // Create a cell for containerName
                    var containerCheckboxCell = document.createElement('td'); // Create a cell for the container checkbox

                    containerCell.textContent = container.containerName; // Populate containerName

                    row.appendChild(containerCell); // Add containerCell to the row

                    // Create a checkbox for the container
                    var isMetadata = false;
                    var containerCheckboxDiv = createCheckboxDiv(container.containerName, selectedMetadata, isMetadata);
                    containerCheckboxCell.appendChild(containerCheckboxDiv);

                    row.appendChild(containerCheckboxCell); // Add containerCheckboxCell to the row
                    tableBody.appendChild(row);
                });
            }
        },

        error: function (error) {
            console.error(error.statusText);
        }
    });
}

function createCheckboxDiv(itemName, selectedItems, isMetadata) {
    var checkboxDiv = document.createElement('div');
    checkboxDiv.className = 'form-check text-center';

    var checkboxInput = document.createElement('input');
    checkboxInput.type = 'checkbox';
    checkboxInput.className = 'form-check-input';

    if (isMetadata) {
        checkboxInput.dataset.metadata = itemName;
    } else {
        checkboxInput.dataset.container = itemName;
    }

    // Check if the item is in selectedItems to set the checkbox as checked
    if (selectedItems.some(item => isMetadata ? item.metadataName === itemName : item.containerName === itemName)) {
        checkboxInput.checked = true;
    }

    checkboxDiv.appendChild(checkboxInput);
    return checkboxDiv;
}


function createCheckboxDiv(metadataName, selectedMetadata, isMetadata) {
    var checkboxDiv = document.createElement('div');
    checkboxDiv.className = 'form-check text-center';
    var checkboxInput = document.createElement('input');
    checkboxInput.type = 'checkbox';
    checkboxInput.className = 'form-check-input';
    checkboxInput.dataset.metadata = metadataName;

    // Check if metadataName is in selectedMetadata to set the checkbox as checked
    if (selectedMetadata.some(item => item.metadataName === metadataName) && isMetadata) {
        checkboxInput.checked = true;
    }

    checkboxDiv.appendChild(checkboxInput);
    return checkboxDiv;
}

function closeCustomAlert() {
    $('#custom-alert-modal').modal('hide');
}
function addMetadata(context) {
    debugger;
    var metadataListSelector = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataList = document.querySelector(metadataListSelector);
    var metaDataTableBodyId = (context === 'add') ? 'metadataTableBody' : 'updateMetaDataTableBody';
    var metaDataModal = (context === 'add') ? 'metadata-modal' : 'updateMetadata-modal';

    // Get the selected metadata from the modal's checkboxes
    var selectedMetadata = [];
    var checkboxes = document.querySelectorAll(`#${metaDataTableBodyId} input[type="checkbox"]`);

    checkboxes.forEach(function (checkbox) {
        if (checkbox.checked) {
            var metadataName = checkbox.dataset.metadata;
            selectedMetadata.push(metadataName);
        }
    });

    // Clear existing rows before populating
    metadataList.innerHTML = '';

    // Add selected metadata rows to the table
    if (selectedMetadata.length < 6) {
        selectedMetadata.forEach(function (metadataName) {
            debugger;
            addMetadataRow(metadataName, context);
            addedMetadata.push(metadataName); // Add to the addedMetadata array            
        });
    } else {
        debugger;
        // Display an alert message when the maximum limit is reached
        $('#custom-alert-modal').modal('show');
        return;
    }

    // Update the generated file name
    updateGeneratedFileName(context);

    if (context === 'update') {
        // Close the update-MetaData-modal after adding metadata
        var metadataModal = document.getElementById(metaDataModal);
        var bootstrapMetadataModal = bootstrap.Modal.getInstance(metadataModal);
        bootstrapMetadataModal.hide();

        // When the "Update-MetaData-modal" is hidden, show the "Update File Type Modal" modal
        $('#updateMetadata-modal').on('hidden.bs.modal', function (event) {
            debugger;
            showUpdateFileTypeModal();
        });
    }

    if (context === 'add') {
        debugger;
        // Close the metadata-modal after adding metadata
        var metadataModal = document.getElementById(metaDataModal);
        var bootstrapMetadataModal = bootstrap.Modal.getInstance(metadataModal);
        bootstrapMetadataModal.hide();

        // When the "Metadata Modal" is hidden, show the "Add New Document" modal
        $('#metadata-modal').on('hidden.bs.modal', function (event) {
            debugger;
            $("#MetaDataError").empty();
            showAddDocumentModal();
        });
    }
}



// Add this variable at the beginning of your script
var userEnteredFileName = '';

// Function to update the generated file name
function updateGeneratedFileName(context) {
    debugger;
    var metadataRowsSelector = (context === 'add') ? '.Meta-Data tr' : '.Update-Meta-Data tr';
    var fileNameInput = (context === 'add') ? document.getElementById('fileName') : document.getElementById('fileName1');

    if (!fileNameInput) {
        console.error('fileNameInput not found.');
        return;
    }

    var rows = document.querySelectorAll(metadataRowsSelector);

    var fileNameParts = [];

    rows.forEach(function (row) {
        debugger;
        var selectedMetadataNames = row.querySelector('.col-8');
        var separatorDropdown = row.querySelector('.dropdown-select');
        var separator = separatorDropdown.value;

        if (selectedMetadataNames && separatorDropdown) {
            selectedMetadataNames = selectedMetadataNames.textContent; // Get the text content
            debugger;
            fileNameParts.push({
                metadataName: selectedMetadataNames,
                separator: separator
            });
        }
    });

    var combinedFileName = userEnteredFileName; // Start with the user-entered text

    fileNameParts.forEach(function (part, index) {
        debugger;
        combinedFileName;
    });

    fileNameInput.value = combinedFileName;
}


// Function to handle user input in the file name field
function handleFileNameInput(input) {
    userEnteredFileName = input.value;
    updateGeneratedFileName('updated'); // You can change 'add' to 'update' if needed
}


function addMetadataRow(metadataName, context) {
    debugger;
    var metaDataContext = (context === 'add') ? '.Meta-Data' : '.Update-Meta-Data';
    var metadataListTable = document.querySelector(metaDataContext);

    var newRow = document.createElement('tr');
    if (context == 'add') {
        newRow.innerHTML = `
                    <td class="col-8">${metadataName}</td>
                    <td>
                        <div class="dropdown text-center">
                                    <select class="dropdown-select" onchange="updateGeneratedFileName('add')">
                                <option value=";">;</option>
                                <option value="_">_</option>
                                <option value=",">,</option>
                                <option value=".">.</option>
                                <option value="-">-</option>
                            </select>
                        </div>
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
    } else {
        newRow.innerHTML = `
        <td class="col-8">${metadataName}</td>
        <td>
            <div class="dropdown text-center">
                <select class="dropdown-select" onchange="updateGeneratedFileName('update')">
                    <option value=";">;</option>
                    <option value="_">_</option>
                    <option value=",">,</option>
                    <option value=".">.</option>
                    <option value="-">-</option>
                </select>
            </div>
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
    }

    metadataListTable.appendChild(newRow);
    rowIndex++;
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
        alert("Cannot remove the only metadata row.");
    }
}

// This function for Saving FileNameType Metadata into database "POST" Method

function saveDocument() {
    debugger;
    var form = $("#addFileTypeForm");

    // Clear existing error messages
    $(".error-message").empty();
    var isValid = true;

    // Validation for Container Type Name
    var fileTypeName = $("#fileName").val();
    if (!fileTypeName) {
        $("#fileNameError").text("File Type Name is required.");
        isValid = false;
    }
    // Clear the Container Type Name error message when input changes
    $("#fileName").on("input", function () {
        $("#fileNameError").empty();
    });

    // Validation for Metadata
    var rows = document.querySelectorAll('.Meta-Data tr');
    if (rows.length === 0) {
        $("#MetaDataError").text("Please select at least one Metadata.");
        isValid = false;
    }

    if (!isValid) {
        return; // Stop the process if validation fails
    }

    // Create an array to hold metadata names and separators
    var metadataNamesList = [];
    rows.forEach(function (row) {
        var selectedMetadataNames = row.querySelector('.col-8').textContent;
        var separatorDropdown = row.querySelector('.dropdown-select');
        var separator = separatorDropdown.value;

        // Push the metadata names and separator as an object into the array
        metadataNamesList.push({
            metadataName: selectedMetadataNames,
            seperator: separator
        });
    });

    // Combine the form data with the metadata array
    var formData = new FormData(form[0]);
    formData.append("SelectedMetadata", JSON.stringify(metadataNamesList));

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
                $('#fileName').val(''); // Reset the text input field
                // Remove only dynamically added metadata rows, not the default row
                $('.Meta-Data tr:not(:first)').remove();
                numRowsAdded = 0;
                // Hide the modal
                $("#addFileNameTypeModal").modal("hide");
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

// this function for show all data into index main page...
function loadDataTable() {
    debugger;
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/CompanyAdmin/Document/GetAllFileNameTypeDocument",
            "type": "GET",
            "dataType": "json",
            "error": function (xhr, status, error) {
                debugger;
                console.error("AJAX Error:", error);
            }
        },
        "columns": [
            {
                "data": "fileNames",
                "width": "20%",
                "render": function (data) {
                    var parts = data.split('_');
                    var firstName = parts[0].trim();
                    return '<div class="expandable-cell">' + firstName + '</div>';
                }
            },
            // ... other columns


            {
                "data": "metaDataTypeList",
                "width": "20%",
                "render": function (data) {


                    return '<div class="expandable-cell">' + data + '</div>';
                }
            },
            {
                "data": "ids",
                "width": "10%",
                "render": function (data, type, row) {
                    debugger;
                    var html = `<div class="form-button-action text-center">`;

                    html += `<button class="btn btn-link text-primary btn-simple-primary btn-lg btn-edit" data-toggle="tooltip" title="Edit File Type MetaData"`;
                    html += `data-original-title="Edit Task" data-bs-toggle="modal" data-bs-target="#editUserModal"`;
                    html += `data-user-id="${data}" data-action="edit" onclick=editFileTypeDoc("/CompanyAdmin/Document/EditFileTypeDoc/${data}")>`;
                    html += `<i class="fa fa-edit"></i></button>`;

                    //html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" data-toggle="tooltip" title="Delete File Type MetaData"`;
                    //html += `data-original-title="Remove" data-bs-toggle="modal" data-bs-target="#deleteUserModal"`;
                    //html += `data-action="delete" onclick=FileTypeDocDeleteBtn("/CompanyAdmin/Document/DeleteFileTypeDoc/${data}")>`;
                    //html += `<i class="fa fa-trash"></i></button>`;

                    html += `<button class="btn btn-link text-danger btn-simple-danger btn-delete" title="Delete User"`;
                    html += `data-original-title="Remove" data-bs-toggle="tooltip" data-bs-target="#deleteUserModal"`;
                    html += `data-user-id="${data}" data-action="delete" onclick="FileTypeDocDeleteBtn(this)">`;
                    html += `<i class="fa fa-trash"></i></button>`;


                    html += `</div > `;
                    return html;
                }
            }
        ]
    })
}

// Function for Delete CompanyUser
function FileTypeDocDeleteBtn(button) {
    debugger;
    var userId = $(button).data('user-id');

    // Set the user ID in the modal form
    $('#deleteUserModal').find('#userId').val(userId);

    // Clear the error messages
    $('.error-message').text('');

    // Open the edit user modal
    $('#deleteUserModal').modal('show');

    // Perform the AJAX request to get metadata details
    $.ajax({
        url: `/CompanyAdmin/Document/GetAllFile?id=${userId}`,  // Include userId as a query parameter
        type: 'GET',

        success: function (userData) {
            debugger;
            // Show a confirmation dialog using SweetAlert
            if (userData) {
                swal({
                    title: `Do you want to delete ${userData.fileNames}?`,
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
                        // Perform the AJAX request to delete the file type document
                        $.ajax({
                            url: `/CompanyAdmin/Document/DeleteFileTypeDoc/${userId}`, // Use userId in the URL
                            type: 'DELETE',
                            success: function (response) {
                                debugger;
                                // Handle the response from the server
                                if (response && response.success) {
                                    toastr.success(response.message);
                                    // Reload the page after a delay of 2 seconds
                                    dataTable.ajax.reload();
                                } else {
                                    // Handle error response
                                    toastr.error(response.message);
                                }
                            },
                            error: function (error) {
                                // Handle AJAX error
                                toastr.error('An error occurred during the request.');
                            }
                        });
                    }
                });
            } else {
                // Handle the case where user data is not available
                toastr.error('User data not found.');
            }
        },
        error: function (error) {
            // Handle AJAX error when fetching user details
            toastr.error('An error occurred while fetching user details.');
        }
    });
}




// This function for Edit File Type "GET" Method....
function editFileTypeDoc(url) {
    debugger;
    var modal = $('#editFileNameTypeModal');

    // Clear the existing metadata rows
    clearMetadataListTable();
    // Perform the AJAX request
    $.ajax({
        url: url,
        type: 'GET',
        success: function (response) {
            debugger;
            // Handle the response from the server
            if (response.success) {
                debugger;
                var fileTypeDoc = response.data;
                $('#id').val(fileTypeDoc.id);
                // Populate the filename input field
                $('#fileName1').val(fileTypeDoc.fileName);

                // Populate the metadata list
                var metadataList = fileTypeDoc.metaDataList;
                metadataList.forEach(function (metaData) {
                    // Call a function to add metadata to the list
                    updateMetaDataRow(metaData);
                    //numRowsAdded++;
                });

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
    function clearMetadataListTable() {
        var metadataListTable = document.querySelector('.Update-Meta-Data');
        metadataListTable.innerHTML = '';
    }
}

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
        <td>
            <div class="dropdown text-center">
                <select class="dropdown-select" onchange="updateGeneratedFileName('update')">
                    <option value=";">;</option>
                    <option value="_">_</option>
                    <option value=",">,</option>
                    <option value=".">.</option>
                    <option value="-">-</option>
                </select>
            </div>
        </td>
        <td class="col-1 text-center align-middle">
            <a class="btn btn-link btn-simple-danger text-danger btn-delete remove-meta" title="Remove Metadata" data-bs-placement="top"
            data-bs-original-title="Remove MetaData" onclick="removeMetadata(this,'update')">
                <i class="fa fa-trash"></i>
            </a>
        </td>
        <td class="col-1 text-center align-middle">
            <a href="#" class="btn btn-md btn-link text-success btn-add" title="Add MetaData" data-bs-placement="top"
                data-bs-original-title="Add MetaData" data-bs-toggle="modal" data-bs-target="#updateMetadata-modal">
                <i class="fa fa-plus-circle plus-icon"></i>
            </a>
        </td>
         `;

    // Set the selected separator based on the metaData object
    var selectElement = newRow.querySelector('.dropdown-select');
    if (metaData.seperator) {
        selectElement.value = metaData.seperator;
    }
    metadataListTable.appendChild(newRow);
}

// This function for Edit File Type "POST" Method....
function updateDocument() {
    debugger;
    var form = $("#editFileTypeForm");
    var id = $("#id").val(); // Get the ID value from the hidden input field

    // Create an array to hold metadata names and separators
    var metadataNamesList = [];

    // Iterate through each row in the metadata list
    var rows = document.querySelectorAll('.Update-Meta-Data tr');
    rows.forEach(function (row) {
        var selectedMetadataNames = row.querySelector('.col-8').textContent;
        var separatorDropdown = row.querySelector('.dropdown-select');
        var separator = separatorDropdown.value;

        // Push the metadata names and separator as an object into the array
        metadataNamesList.push({
            metadataName: selectedMetadataNames,
            seperator: separator
        });
    });

    // Combine the form data with the metadata array
    var formData = new FormData(form[0]);
    formData.append("Id", id); // Append the ID to the FormData
    formData.append("SelectedMetadata", JSON.stringify(metadataNamesList));

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
                numRowsAdded = 0;
                var metadataListTable = document.querySelector('.Update-Meta-Data');
                metadataListTable.innerHTML = '';
                // Hide the modal
                $("#editFileNameTypeModal").modal("hide");
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

// Call the function to set the initial values on page load
setInitialValues();

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
                        <div class="dropdown text-center">
                            <select id="separatorDropdown_Add" class="dropdown-select" onchange="updateSelectedSeparator(this)">
                                <option value=";">;</option>
                                <option value="_">_</option>
                                <option value=",">,</option>
                                <option value=".">.</option>
                                <option value="-">-</option>
                            </select>
                        </div>
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

// Download All FileType MetaData in Excel Format...
function exportExcel() {
    debugger;
    window.location.href = '/CompanyAdmin/Document/ExportExcelFileType';
}

// Download All FileType Metadata in PDF Format...
function exportPdf() {
    debugger;
    window.location.href = '/CompanyAdmin/Document/ExportPdfFileType';
}