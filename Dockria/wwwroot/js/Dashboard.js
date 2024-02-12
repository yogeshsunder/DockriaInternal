// Initially hide all buttons except save company"
$(document).ready(function () {
    $("#DeletecompanyForm, #ClearcompanyForm, #UpdatecompanyForm, #downloadcompanyForm").hide();
});

// This function for Edit Get Company Admin details....
function onEditClick(num) {

    showEditButtons();
    hideEditButtons();

    $.ajax({
        type: 'GET',
        url: '/Admin/Dashboard/EditFormPartial',
        data: { id: num },
        dataType: 'json',
        success: function (result) {

            console.log(result);
            // Update the form fields with the retrieved data
            $('#id').val(result.admin.id);
            $('#adminName').val(result.admin.adminName);
            $('#name').val(result.admin.name);
            $('#adminEmail').val(result.admin.adminEmail);
            $('#address').val(result.admin.address);
            $('#adminDesignation').val(result.admin.adminDesignation);
            $('#officialEmail').val(result.admin.officialEmail);
            $('#adminPhoneNumber').val(result.admin.adminPhoneNumber);
            $('#phoneNumber').val(result.admin.phoneNumber);
            $('#registrationNumber').val(result.admin.registrationNumber);
            $('#pinNumber').val(result.admin.pinNumber);
            $('#storageSpace').val(result.admin.storageSpace);
            $('#invoiceMail').val(result.admin.invoiceMail);
            $('#reciveEmail').val(result.admin.reciveEmail);
            $('#dateFrom').val(result.admin.dateFrom);
            $('#dateTo').val(result.admin.dateTo);

            // Update the file input field with the file names
            updateLink(result.admin.fileNames);

            updateDropdownList(result.admin.subscriptionName, result.subscriptionList);

        },
        error: function () {
            alert('An error occurred while retrieving the Admin data.');
        }
    });


    function showEditButtons() {
        $("#DeletecompanyForm, #ClearcompanyForm, #UpdatecompanyForm, #downloadcompanyForm").show();
    }
    function hideEditButtons() {
        $("#savecompanyForm").hide();
    }
    function updateLink(fileNames) {

        var fileInput = $('#fileInput');
        var uploadLink = $('#uploadLink');

        uploadLink.empty(); // Clear the existing links

        if (fileNames && fileNames.length > 0) {
            for (var i = 0; i < fileNames.length; i++) {
                var fileName = fileNames[i];
                var link = $('<a>').attr('href', '#').text(fileName).addClass('file-link');
                uploadLink.append(link);
            }
        } else {
            uploadLink.text('No file chosen');
        }
    }

    function updateDropdownList(selectedValue, options) {

        var dropdown = $('#subscriptionList');
        dropdown.empty();

        $.each(options, function (index, option) {
            var optionElement = $('<option>').val(option.value).text(option.text);
            if (option.text === selectedValue) {
                optionElement.prop('selected', true);
            }
            dropdown.append(optionElement);
        });
    }
}

$(document).ready(function () {

    $('#uploadLink').click(function (e) {
        e.preventDefault();
        $('#fileInput').click();
    });

    $('#fileInput').change(function () {
        var filenames = Array.from(this.files).map(file => file.name).join(', ');
        $('#uploadLink').text(filenames);
    });

    $('#btnUpload').click(function () {
        var files = $('#fileInput')[0].files;
        // Perform your upload logic here
    });
});


// This funtion for Update Company Admin details Post Method.....
function onUpdateClick(formId) {

    $('#updateAdminForm').validate();
    if (!$('#updateAdminForm').valid()) {
        return;
    }
    var form = $(formId);
    var url = form.attr('action');
    var serializedData = form.serialize();

    // Get the existing file names from the upload link
    var existingFileNames = $('#uploadLink').text().split(', ');

    // Get the file names from the file input field
    var fileInput = $('#fileInput')[0];
    var fileNames = [];
    if (fileInput.files && fileInput.files.length > 0) {
        for (var i = 0; i < fileInput.files.length; i++) {
            fileNames.push(fileInput.files[i].name);
        }
    } else {
        // If no new files are selected, use the existing file names
        fileNames = existingFileNames;
    }

    // Append the file names to the serialized data if there are files
    if (fileNames.length > 0) {
        serializedData += '&fileNames=' + fileNames.join(',');
    }

    // Get the value of the subscriptionList field
    var subscriptionList = $('#subscriptionList').val();

    // Assuming the ID of the input field is 'subscriptionName'
    var subscriptionName = $('#subscriptionName').val();

    // Get the values of the additional fields
    var dateFrom = $('#dateFrom').val();
    var dateTo = $('#dateTo').val();
    var registrationNumber = $('#registrationNumber').val();
    var pinNumber = $('#pinNumber').val();
    var storageSpace = $('#storageSpace').val();
    var reciveEmail = $('#reciveEmail').val();
    var invoiceMail = $('#invoiceMail').val();

    // Append the additional fields to the serialized data
    serializedData +=
        '&subscriptionList=' + encodeURIComponent(subscriptionList) +
        '&subscriptionName=' + encodeURIComponent(subscriptionName) +
        '&dateFrom=' + encodeURIComponent(dateFrom) +
        '&dateTo=' + encodeURIComponent(dateTo) +
        '&registrationNumber=' + encodeURIComponent(registrationNumber) +
        '&pinNumber=' + encodeURIComponent(pinNumber) +
        '&storageSpace=' + encodeURIComponent(storageSpace) +
        '&reciveEmail=' + encodeURIComponent(reciveEmail) +
        '&invoiceMail=' + encodeURIComponent(invoiceMail);

    $.ajax({
        type: 'POST',
        url: '/Admin/Dashboard/EditFormPartial',
        data: serializedData,
        success: function (response) {
            // Handle successful response from server
            $('#successModalBody').html('Admin data updated successfully');
            $('#successModal').modal('show');
            // Clear the form
            $('#updateAdminForm')[0].reset();
        },
        error: function (xhr, status, error) {
            // Handle error response from server
            $('#errorModalBody').html('Error updating Admin data: ' + error);
            $('#errorModal').modal('show');
        }
    });
}
// Delete Button function from table delete button....
function deleteBtn(formId) {

    var id = parseInt(formId);

    // Show confirmation message in the modal
    $('#confirmationModalBody').html('Do you want to delete this Company?');
    $('#confirmationModal').modal('show');

    // Attach a click event to the Confirm button inside the modal
    $('#confirmBtn').off('click').on('click', function () {
        // Execute the delete operation if confirmed
        $.ajax({
            url: '/Admin/Dashboard/Delete/' + id,
            type: 'DELETE',
            success: function (result) {
                if (result.success) {
                    // Show success message in the modal
                    $('#successModalBody').html(result.message);
                    $('#successModal').modal('show');
                    setInterval('location.reload()', 1000);
                } else {
                    // Show error message in the modal
                    $('#errorModalBody').html('An error occurred while deleting the Data.');
                    $('#errorModal').modal('show');
                }
            },
            error: function (xhr, status, error) {
                // Show error message in the modal
                $('#errorModalBody').html('An error occurred while deleting the Data.');
                $('#errorModal').modal('show');
            }
        });

        // Close the confirmation modal
        $('#confirmationModal').modal('hide');
    });
}

// Add change event handler to the dropdown
$('#subscriptionList').on('change', function () {

    var selectedValue = $(this).val();
    $(this).data('selected-value', selectedValue);
});

// Add change event handler to the dropdown
$('#subscriptionList').on('change', function () {

    var selectedValue = $(this).val();
    $(this).data('selected-value', selectedValue);
});
// This function for Save Company Admin Details Post Method......
function onSaveClick(formId) {

    if (formId != null) {
        $('#updateAdminForm').validate();
        if (!$('#updateAdminForm').valid()) {
            return;
        }
    }
    var formData = new FormData();
    var files = $('#fileInput')[0].files;

    // Check file limit
    if (files.length > 5) {
        // Show error message in the modal
        $('#errorModalBody').html('Maximum file upload limit is 5');
        $('#errorModal').modal('show');
        return;
    }

    // Append files to form data
    for (var i = 0; i < files.length; i++) {
        formData.append('files', files[i]);
    }

    // Append other form data fields
    formData.append('Id', $('#id').val());
    formData.append('AdminName', $('#adminName').val());
    formData.append('Name', $('#name').val());
    formData.append('AdminEmail', $('#adminEmail').val());
    formData.append('Address', $('#address').val());
    formData.append('AdminDesignation', $('#adminDesignation').val());
    formData.append('OfficialEmail', $('#officialEmail').val());
    formData.append('AdminPhoneNumber', $('#adminPhoneNumber').val());
    formData.append('PhoneNumber', $('#phoneNumber').val());
    formData.append('RegistrationNumber', $('#registrationNumber').val());
    formData.append('PinNumber', $('#pinNumber').val());
    formData.append('StorageSpace', $('#storageSpace').val());
    formData.append('InvoiceMail', $('#invoiceMail').val());
    formData.append('ReciveEmail', $('#reciveEmail').val());
    formData.append('DateFrom', $('#dateFrom').val());
    formData.append('DateTo', $('#dateTo').val());

    // Handle FileNames
    var fileNames = [];
    for (var i = 0; i < files.length; i++) {
        fileNames.push(files[i].name);
    }
    formData.append('FileNames', fileNames.join(','));

    // Handle SubscriptionName
    var subscriptionName = $('#subscriptionList').val();
    formData.append('SubscriptionName', subscriptionName);
    var subscriptionList = 5;
    formData.append('SubscriptionList', subscriptionList);

    $.ajax({
        type: 'POST',
        url: '/Admin/Dashboard/Create',
        data: formData,
        contentType: false,
        processData: false,
        success: function (result) {

            if (result.success) {

                // Show success message in the modal
                $('#successModalBody').html('Admin saved successfully!');
                $('#successModal').modal('show');
                // window.location.href = '/Admin/Dashboard/Details';
                $('#addAdminModal').modal('hide');
                // Update the Admin list with the new Admin
                $('#companies-list').load('/Admin/Dashboard/CompaniesList');
                // Clear the form
                $('#updateAdminForm')[0].reset();
                setInterval('location.reload()', 1000);

            } else {
                // Show error message in the modal
                $('#errorModalBody').html(result.message);
                $('#errorModal').modal('show');
            }
        },
        error: function () {

            // Show error message in the modal
            $('#errorModalBody').html('An error occurred while saving the Admin.');
            $('#errorModal').modal('show');
        }
    });
}
function formatDate(dateString) {

    return dateString; 
}
function onDeleteClick(formId) {

    var adminId = $('#id').val();
    var id = parseInt(adminId);

    if (!id || id === 0) {
        $('#errorModalBody').html('Please click Edit Button on Company Admin list first then click Delete Button.');
        $('#errorModal').modal('show');
        return;
    }
    $('#confirmationModalBody').html('Do you want to delete this admin?');
    $('#confirmationModal').modal('show');
    $('#confirmBtn').off('click').on('click', function () {
        $.ajax({
            url: '/Admin/Dashboard/Delete/' + id,
            type: 'DELETE',
            dataType: 'json',
            success: function (result) {

                if (result.success) {
                    $('#successModalBody').html(result.message);
                    $('#successModal').modal('show');
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                } else {
                    $('#errorModalBody').html(result.message);
                    $('#errorModal').modal('show');
                }
            },
            error: function (xhr, status, error) {
                $('#errorModalBody').html('An error occurred while deleting the admin. ' + error);
                $('#errorModal').modal('show');
            }
        });

        $('#confirmationModal').modal('hide');
    });
}

function onClearForm() {
    $('#updateAdminForm')[0].reset();
    location.reload();
    
}


function downloadInvoice() {
    let id = $('#id').val();
    $.ajax({
        type: 'GET',
        url: '/Admin/Dashboard/EditFormPartial',
        data: { id: id },
        dataType: 'json',
        success: function (result) {
            html2canvas($('#downloadcompanyForm')[0]).then(function (canvas) {
                var pdf = new jsPDF('p', 'pt', 'letter');
                pdf.setFont('helvetica');
                pdf.setFontSize(12);
                pdf.setFontType('bold');
                pdf.setTextColor(44, 62, 80);
                pdf.text('Company Invoice', 20, 30);
                pdf.setFontType('normal');
                pdf.setTextColor(0, 0, 0);
                let yPos = 60;

                // Function to capitalize the first letter and add spaces between words
                function formatPropertyName(prop) {
                    return prop.replace(/([a-zA-Z])([A-Z])/g, '$1 $2')
                        .replace(/\b\w/g, match => match.toUpperCase());
                }

                for (let prop in result.admin) {
                    let propName = formatPropertyName(prop);
                    let propValue = result.admin[prop];

                    pdf.text(propName + ': ' + propValue, 20, yPos);
                    yPos += 20;
                }
                pdf.save('Invoice.pdf');
            });
        },
        error: function () {
            alert('An error occurred while fetching dynamic data.');
        }
    });
}
