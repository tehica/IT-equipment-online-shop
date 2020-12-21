function readURL(input) {
    // check is this input file
    if (input.files && input.files[0]) {
        let reader = new FileReader();

        reader.onload = function (event) {
            $("img#imgPreview").attr("src", event.target.result).width(200).height(200);
        };

        reader.readAsDataURL(input.files[0]);
    }
}