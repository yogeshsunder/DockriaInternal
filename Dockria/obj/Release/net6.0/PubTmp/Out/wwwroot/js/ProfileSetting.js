function onUpdateClick(data) {
    debugger;
    if (data != null) {
        $('#updateProfileForm').validate();
        if (!$('#updateProfileForm').valid()) {
            return;
        }
    }

    var formData = new FormData();

    // Append the image file to the FormData object
    var imageFile = $('#image-upload')[0].files[0];
    formData.append('profileImage', imageFile);

    // Append the rest of the form data to the FormData object
    var id = $('#userId').val();
    formData.append('Id', id ? id : 0);

    formData.append('FirstName', $('#firstName').val());
    formData.append('LastName', $('#lastName').val());
    formData.append('NickName', $('#nickName').val());
    formData.append('About', $('#about').val());
    formData.append('Address', $('#address').val());
    formData.append('Designation', $('#designation').val());
    formData.append('Email', $('#email').val());
    formData.append('Mobile', $('#mobile').val());
    formData.append('TwitterAct', $('#twitterAct').val());
    formData.append('FacebookAct', $('#facebookAct').val());
    formData.append('GooglePlusAct', $('#googlePlusAct').val());
    formData.append('LinkedAct', $('#linkedAct').val());

    formData.append('__RequestVerificationToken', $('input[name="__RequestVerificationToken"]').val());

    $.ajax({
        type: 'POST',
        url: '/ProfileManagement/Profile/UpdateProfile',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            debugger;
            if (result.success) {
                // Display success message in the success modal
                $("#successModalBody").text(result.message);
                $("#successModal").modal("show");
                // Example: Reload the page after 1000 milliseconds
                setInterval('location.reload()', 1000);
            } else {
                // Display error message in the error modal
                $("#errorModalBody").text(result.message);
                $("#errorModal").modal("show");
            }
        },
        error: function () {
            debugger;
            // Display a generic error message in the error modal
            $("#errorModalBody").text('An error occurred while Updating the Profile....');
            $("#errorModal").modal("show");
        }
    });
};


function changeImage() {
    document.getElementById("image-upload").click();
}

function showImagePreview(event) {
    var file = event.target.files[0];
    var reader = new FileReader();

    reader.onload = function (e) {
        var personImage = document.getElementById('person-image');
        personImage.src = e.target.result;
    };

    reader.readAsDataURL(file);
}

function deleteImage() {
    document.getElementById("person-image").src = "~/adminAssets/assets/images/users/male/24.jpg";
}