$(document).ready(function () {
    $("#Time").click(() => {
        let location = window.location.href;
                                   //send everything after the ?
        let params = getAllParams(window.location.search.substring(1));
        let sort = params.find(p => p.name == "sort");
        if (sort == undefined)
        {
            params.push(new Param("sort", "asc"));
        }
        else {
            if (sort.value == "desc") {
                sort.value="asc";
            }
            else {
                sort.value = "desc";
            }

        }
        location = getParamsInLocation(location, params);
        window.location.href = location;
    });
});