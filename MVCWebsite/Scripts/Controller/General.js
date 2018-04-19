$(document).ready(() => {

    $(".table-title").children(":not(.non-sortable)").click(e => {
        let element = e.currentTarget;
        let location = window.location.href;

        //switch sorting
        let sort = location.includes("sort=desc") ? "asc" : "desc";
        let column = element.innerHTML.trim();


        location = location.split('?')[0] + "?sort=" + sort + "&column=" + column;
        window.location.href = location;
    });
})