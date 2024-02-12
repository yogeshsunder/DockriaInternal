////var currentId = 0;
////$(document).ready(function () {
////    loadValues();

////    // Event handler for checkboxes
////    $("#tblValues tbody").on("click", "input[type='checkbox']", function () {
////        debugger;
////        var checkboxes = $("#tblValues tbody input[type='checkbox']");
////        checkboxes.not(this).prop("checked", false);
////    });
////});

////// This Function for AddFileTypeMetadata "POST" method...
////function saveMetadata() {
////    debugger;
////    var form = $("#metadata-form");

////    // Clear existing error messages
////    $(".error-message").empty();
////    var isValid = true;

////    // Validation for Document Type Name
////    var metaDataName = $("#metadata-name").val();
////    if (!metaDataName) {
////        $("#metaDataNameError").text("MetaData Name is required.");
////        isValid = false;
////    }

////    // Clear the Document Type Name error message when input changes
////    $("#metadata-name").on("input", function () {
////        $("#metaDataNameError").empty();
////    });

////    // Add event listener to metadata type dropdown
////    $("#metadataType").on("change", function () {
////        var selectedValue = $(this).val();
////        if (!selectedValue) {
////            $("#metaDataTypeError").text("METADATA DATA TYPE is required.");
////        } else {
////            $("#metaDataTypeError").empty();
////        }
////    });

////    // Validation for User Group
////    var selectedUserGroups = $("#metaGroupList option:selected:not([disabled])").length;
////    if (selectedUserGroups === 0) {
////        $("#UserGroupError").text("At least one User Group must be selected.");
////        isValid = false;
////    }

////    // Clear the User Group error message when a new option is selected
////    $("#metaGroupList").on("change", function () {
////        $("#UserGroupError").empty();
////    });

////    if (!isValid) {
////        return; // Stop the process if validation fails
////    }

////    // Get selected user group list from multi-select dropdown
////    var selectedUserGroups = [];
////    $("#metaGroupList option:selected").each(function () {
////        selectedUserGroups.push($(this).val());
////    });

////    // Retrieve metadata name and type
////    var metaDataTypeName = $("#metadata-name").val();
////    var metaDataDataType = $("#metadataType").val();

////    // Combine the form data with the Selected GroupNames and metadata info
////    var formData = new FormData(form[0]);
////    formData.append("SelectedUserGroups", JSON.stringify(selectedUserGroups));
////    formData.append("MetaDataTypeName", metaDataTypeName); // Use "MetaDataTypeName" as key
////    formData.append("MetaDataDataType", metaDataDataType);

////    $.ajax({
////        url: form.attr("action"),
////        type: "POST",
////        data: formData,
////        dataType: "json",
////        contentType: false, // Important for sending FormData
////        processData: false, // Important for sending FormData
////        success: function (response) {
////            debugger;
////            if (response.success) {
////                debugger;
////                // Clear the form
////                form[0].reset();
////                // Hide the modal
////                $("#addMetaDataTypeModal").modal("hide");

////                var selectedMetadata = [];
////                $(".Meta-Data tr").each(function () {
////                    var metadataName = $(this).attr("data-metadata-name");
////                    var isChecked = $(this).find("input[type='checkbox']").prop("checked");
////                    selectedMetadata.push({ metadataName: metadataName, isChecked: isChecked });
////                });

////                fetchMetadata('metaDataBody', selectedMetadata);
////            } else {
////                // Document save failed, handle the error (e.g., show an error message)
////                console.error("Document save failed:", response.errorMessage);
////            }
////        },
////        error: function (error) {
////            console.error("An error occurred:", error);
////        }
////    });
////}

////function loadValues() {
////    debugger;
////    // Fetch all data again from the server and update the table
////    $.ajax({
////        url: '/CompanyAdmin/ContainerType/GetAllValues',
////        type: 'GET',
////        dataType: 'json',
////        success: function (data) {
////            debugger;
////            console.log(data);
////            if (data && data.length > 0) {
////                // Iterate through the data and append rows to the table
////                data.forEach(function (item) {
////                    debugger;
////                    appendToTable(item.id, item.valueId, item.name); // Use correct property names
////                    currentId = item.valueId;
////                });
////            }
////        },
////        error: function (error) {
////            console.error("An error occurred:", error);
////        }
////    });
////};





////function addValueButton() {
////    debugger;
////    // Generate a unique ID
////    var valueId = generateUniqueId() + "";

////    // Prompt the user to enter a name
////    var name = prompt("Enter a Name:");

////    // Check if a Name was entered and not canceled
////    if (name !== null) {
////        var data = {
////            ValueId: valueId,
////            Name: name
////        };
////        console.log("Data to send:", data);
////        // Send the name to the server using AJAX
////        $.ajax({
////            url: '/CompanyAdmin/ContainerType/SaveValues',
////            type: 'POST',
////            contentType: 'application/json',
////            data: JSON.stringify(data), // Send the name as JSON data
////            dataType: 'json',
////            success: function (response) {
////                debugger;
////                if (response.success) {
////                    alert(response.message);
////                    // Clear the existing table rows
////                    var tableBody = document.querySelector("#tblValues tbody");
////                    tableBody.innerHTML = '';

////                    var listValue = response.data;
////                    // Append the new value's Id, ValueId, and Name to the table
////                    listValue.forEach(function (data) {
////                        appendToTable(data.id, data.valueId, data.name);
////                    });

////                    // You can perform additional actions here upon successful save
////                } else {
////                    alert('Save failed: ' + response.message);
////                }
////            },
////            error: function (error) {
////                console.error("An error occurred:", error);
////            }
////        });
////    }
////}

////function generateUniqueId() {
////    debugger;
////    currentId++; // Increment the current ID
////    return currentId;
////}

////// Function to append a new row to the table with ValueId and Name
////function appendToTable(id, valueId, name) {
////    debugger;
////    // Find the table body
////    var tableBody = document.querySelector("#tblValues tbody");

////    // Create a new table row
////    var newRow = document.createElement("tr");

////    // Add cells for ValueId and Name
////    newRow.innerHTML = `
////        <td><input type="checkbox"></td>
////        <td style="display: none;"><input type="hidden" value="${id}"></td>
////        <td>${valueId}</td> 
////        <td>${name}</td>       
////    `;

////    // Append the new row to the table
////    tableBody.appendChild(newRow);
////}




//$("#editValueButton").on("click", function () {
//    debugger;

//    // Get all checkboxes within the table body
//    var checkboxes = $("#tblValues tbody input[type='checkbox']");
//    // Find the checked checkbox
//    var selectedCheckbox = checkboxes.filter(":checked");

//    // Check if any checkbox is checked
//    if (selectedCheckbox.length === 0) {
//        alert("Please select a row to edit.");
//        return;
//    }

//    // Get the closest row to the selected checkbox
//    var selectedRow = selectedCheckbox.closest("tr");

//    // Assuming the ID is in the third column (adjust as needed)
//    var idInput = selectedRow.find("td:nth-child(2) input");

//    var id = idInput.val().trim();

//    // Send an AJAX request to fetch the data
//    $.ajax({
//        url: '/CompanyAdmin/ContainerType/EditValue',
//        type: 'GET',
//        data: { id: id }, // Pass the ID as a parameter
//        dataType: 'json',
//        success: function (data) {
//            debugger;
//            if (data) {
//                // Handle the response data here
//                console.log("Received data:", data);

//                // Prompt the user to enter a new value with the pre-filled value from the data
//                var editedValue = prompt("Enter a Edit value:", data.name);

//                // Check if the user canceled or entered an empty value
//                if (editedValue === null) {
//                    alert("Edit canceled.");
//                } else if (editedValue.trim() === "") {
//                    alert("Empty value entered. No changes made.");
//                } else {
//                    // Update the table cell with the edited value
//                    selectedRow.find("td:nth-child(4)").text(editedValue);

//                    // Send a POST request to save the edited value
//                    $.ajax({
//                        url: '/CompanyAdmin/ContainerType/EditValue',
//                        type: 'POST',
//                        data: { id: id, editedValue: editedValue },
//                        success: function (response) {
//                            debugger;
//                            console.log(response);
//                            // Show an alert with the success message
//                            alert(response);
//                        },
//                        error: function (error) {
//                            console.error("An error occurred while saving:", error);
//                        }
//                    });
//                }
//            } else {
//                alert('No data found.');
//            }
//        },
//        error: function (error) {
//            console.error("An error occurred:", error);
//        }
//    });
//});

////// Add a click event handler for the delete button
////$("#deleteValueButton").on("click", function () {
////    debugger;
////    // Get all checkboxes within the table body
////    var checkboxes = $("#tblValues tbody input[type='checkbox']");
////    // Find the checked checkbox
////    var selectedCheckbox = checkboxes.filter(":checked");

////    // Check if any checkbox is checked
////    if (selectedCheckbox.length === 0) {
////        alert("Please select a row to edit.");
////        return;
////    }

////    // Get the closest row to the selected checkbox
////    var selectedRow = selectedCheckbox.closest("tr");

////    // Assuming the ID is in the third column (adjust as needed)
////    var idInput = selectedRow.find("td:nth-child(2) input");

////    var id = idInput.val().trim();

////    // Confirm with the user before deleting
////    if (confirm("Are you sure you want to delete this record?")) {
////        // Send an AJAX request to delete the record
////        $.ajax({
////            url: '/CompanyAdmin/ContainerType/DeleteValue',
////            type: 'POST',
////            data: { id: id }, // Pass the ID as a parameter
////            success: function (response) {
////                debugger;
////                console.log(response);

////                // Remove the row from the table upon successful deletion
////                selectedRow.remove();

////                // Show an alert with the success message
////                alert(response);
////            },
////            error: function (error) {
////                console.error("An error occurred while deleting:", error);
////                alert("Error deleting the record.");
////            }
////        });
////    }
////});
////function ValueForm() {
////    debugger;
////    var fileInput = document.getElementById("userDataFile");
////    var file = fileInput.files[0];
////    var formData = new FormData();
////    formData.append("formFile", file);

////    $.ajax({
////        url: "/CompanyAdmin/ContainerType/BulkValueAddition",
////        type: "POST",
////        data: formData,
////        contentType: false,
////        processData: false,
////        success: function (response) {
////            debugger;
////            if (response.success) {
////                debugger;
////                var successMessages = response.successMessage || [];
////                var errorMessages = response.errorMessage || [];

////                if (successMessages.length > 0 || errorMessages.length > 0) {
////                    var errorMessageElement = document.getElementById('bulkErrorMessage');
////                    errorMessageElement.innerHTML = ''; // Clear previous messages

////                    if (successMessages.length > 0) {
////                        var successHeading = document.createElement('h4');
////                        successHeading.textContent = 'Success Messages';
////                        errorMessageElement.appendChild(successHeading);

////                        var successMessageList = document.createElement('ul');
////                        successMessageList.classList.add('success-message-list');

////                        for (var i = 0; i < successMessages.length; i++) {
////                            var messageItem = document.createElement('li');
////                            messageItem.textContent = successMessages[i];
////                            successMessageList.appendChild(messageItem);
////                        }

////                        errorMessageElement.appendChild(successMessageList);
////                    }

////                    if (errorMessages.length > 0) {
////                        var errorHeading = document.createElement('h4');
////                        errorHeading.textContent = 'Error Messages';
////                        errorMessageElement.appendChild(errorHeading);

////                        var errorMessageList = document.createElement('ul');
////                        errorMessageList.classList.add('error-message-list');

////                        for (var i = 0; i < errorMessages.length; i++) {
////                            var messageItem = document.createElement('li');
////                            messageItem.textContent = errorMessages[i];
////                            errorMessageList.appendChild(messageItem);
////                        }

////                        errorMessageElement.appendChild(errorMessageList);
////                    }

////                    // Hide the "Bulk User Addition" modal
////                    $('#bulkUserModal').modal('hide');

////                    var errorModal = new bootstrap.Modal(document.getElementById('bulkModal'));
////                    errorModal.show();
////                }
////                // Reload the CompanyAdmin/CompanyAdmin page after a delay of ten seconds
////                setTimeout(function () {
////                    window.location.href = '/CompanyAdmin/ContainerType/ContainerIndex';
////                }, 5000);
////            } else {
////                debugger;
////                var errorMessage = "";

////                if (response.errorMessage && response.errorMessage.length > 0) {
////                    var errorMessageElement = document.getElementById('bulkErrorMessage');
////                    errorMessageElement.innerHTML = ''; // Clear previous messages

////                    var errorMessageHeading = document.createElement('h4');
////                    errorMessageHeading.textContent = 'Error Messages';
////                    errorMessageHeading.classList.add('error-message-list');
////                    errorMessageElement.appendChild(errorMessageHeading);

////                    var errorMessageList = document.createElement('ul');

////                    for (var i = 0; i < response.errorMessage.length; i++) {
////                        var messageItem = document.createElement('li');
////                        messageItem.textContent = response.errorMessage[i];
////                        errorMessageList.appendChild(messageItem);
////                    }

////                    errorMessageElement.appendChild(errorMessageList); // Add the unordered list to the error message element
////                } else {
////                    errorMessage = 'Failed to create user. Please try again.';
////                }

////                // Hide the "Bulk User Addition" modal
////                $('#bulkUserModal').modal('hide');

////                var errorModal = new bootstrap.Modal(document.getElementById('bulkModal'));
////                errorModal.show();

////                // Reload the CompanyAdmin/CompanyAdmin page after a delay of ten seconds
////                setTimeout(function () {
////                    window.location.href = '/CompanyAdmin/ContainerType/ContainerIndex';
////                }, 5000);
////            }
////        },
////        error: function (error) {
////            if (error.responseJSON && error.responseJSON.errors && error.responseJSON.errors.length > 0) {
////                var errorMessage = error.responseJSON.errors.join('<br>');
////                showErrorMessage(errorMessage);
////            } else {
////                showErrorMessage('An error occurred while creating the user. Please try again later.');
////            }
////        }
////    });
////}