function setImage(personImg) {
    var imageFile = document.getElementById("imageName");
    if ((personImg === "" || personImg == null) && imageFile.files.length == 0) {
        const checkedGender = document.querySelector('input[name = "gender"]:checked');
        var Img = document.getElementById("imagePreview");

        if (checkedGender) {
            if (checkedGender.value === "0") {
                Img.src = "/images/person-placeholder/Male.png";
            }
            else {
                Img.src = "/images/person-placeholder/Female.png";
            }
        }
    }
}