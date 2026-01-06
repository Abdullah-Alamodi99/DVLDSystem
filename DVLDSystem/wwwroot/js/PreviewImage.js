function validateImage(input) {
    const file = input.files[0];

    if (!file) return;

    const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];
    const maxSizeMB = 5;
    const maxSize = maxSizeMB * 1024 * 1024;

    // Type validation
    if (!allowedTypes.includes(file.type)) {
        alert('يرجى أختيار صورة بصيغة (PNG, JPEG أو JPG)');
        input.value = '';
        return;
    }

    // Size validation
    if (file.size > maxSize) {
        alert('حجم الصورة يجب أن يكون أقل من ' + maxSizeMB + 'MB');
        input.value = '';
        return;
    }
}
function showImagePreview(event) {
    const imageFile = document.getElementById("imageName");
    var removePriviewImgButton = document.getElementById("removePreviewImg");
    if (imageFile && imageFile.files && imageFile.files.length > 0) {
        const imagePreview = document.getElementById("imagePreview");
        const file = event.target.files[0];
        if (file) {
            
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = function (e) {
                imagePreview.src = e.target.result;
                removePriviewImgButton.style.display = "block";
            }
        }
        else {
            alert('Please select a valid image file.');
        }
    }
}

function removeImage() {
    var imageFile = document.getElementById("imageName");
    var checkedGender = document.querySelector('input[name = "gender"]:checked');
    var removePriviewImgButton = document.getElementById("removePreviewImg");
    imageFile.value = "";
    removePriviewImgButton.style.display = "none";

    var imagePreview = document.getElementById("imagePreview");
    if (checkedGender) {
        if (checkedGender.value === "0") {
            imagePreview.src = "/images/person-placeholder/Male.png";
        }
        else {
            imagePreview.src = "/images/person-placeholder/Female.png";
        }
    }
}