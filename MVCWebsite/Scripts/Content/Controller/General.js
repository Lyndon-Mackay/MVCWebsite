$(document).ready(() => {
    $("input[type=checkbox]");
    {
        let locationParamsString = window.location.search.substr(1);
        let params = getAllParams(locationParamsString);
        for (var i = 0; i < params.length; i++) {
            //console.log("in " + params[i].name);
            //console.log(`is checkbox ${params[i].name} ` + $(`input[name=${params[i].name}]`).is(":checkbox"));
            if ($(`input[name=${params[i].name}]`).is(":checkbox")) {
                $(`input[name=${params[i].name}]`).prop('checked', params[i].value);
            }
        }
    }
    //clicking on a sortable column change sort and column otherwise as normal
    $(".table-title").children(":not(.non-sortable)").click(e => {
        let element = e.currentTarget;
        let location = window.location.href;
        //gets url after ?
        let locationParamsString = window.location.search.substr(1);
        let params = getAllParams(locationParamsString);
        let sort = params.find(p => p.name == "sort");
        if (sort == undefined) {
            sort = new Params("sort", "asc");
            params.push(sort);
        }
        console.log("params" + params);
        //switch sorting
        sort.value = sort.value == "desc" ? "asc" : "desc";
        let column = element.innerHTML.trim();
        params.push(new Params("column", column));
        location = location.split('?')[0] + "?" + params.join("&").substr(0);
        //beaware of location switching clearing the console
        window.location.href = location;
    });
    //make sure at least one checkbox is checked
    $("#SearchForm").submit(e => {
        let checked = $("input[type=checkbox]:checked").length;
        if (checked == 0) {
            if (!$("#formErrorText").length) {
                let p = $("<p>", { id: "formErrorText", "class": "error" });
                p.append("At least one checkbox must be selected");
                $("#SearchForm").append(p);
            }
            else {
                $("#formErrorText").fadeOut(100).fadeIn(100);
            }
            return false;
        }
    });
});
class Params {
    constructor(name, value) {
        this.name = name;
        this.value = value;
    }
    toString() {
        if (this.name.length > 1 && this.value.length > 1) {
            return this.name + "=" + this.value;
        }
        return "";
    }
}
function getAllParams(paramsString) {
    let params = new Array();
    let keyValueString = paramsString.split('&');
    //let statement did not work
    for (var i = 0; i < keyValueString.length; i++) {
        let split = keyValueString[i].split('=');
        params.push(new Params(split[0], split[1]));
    }
    return params;
}
//# sourceMappingURL=General.js.map