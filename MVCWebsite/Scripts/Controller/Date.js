$(document).ready(function () {
    $("#Time").click(() => {
        let location = window.location.href
        //currently not scalable but only one property
        if (location.endsWith("/dates")) {
            location = "?sort=desc";
        }
        else {
            if (location.includes("sort=desc")) {
                location = location.replace("sort=desc", "sort=asc")
            }
            else {
                location = location.replace("sort=asc", "sort=desc")
            }

        }
        window.location.href = location;
    });
    // do stuff when DOM is ready
});