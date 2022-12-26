$(function () {
    /* Preview selected image */
    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $("img#imgpreview")
                    .attr("src", e.target.result)
                    .width(80)
                    .height(80)
                    .show();
            }

            reader.readAsDataURL(input.files[0]);
        }
        else {
            $("img#imgpreview").hide();
        }
    }

    $("#imageUpload").change(function () {
        readURL(this);
    });    
});