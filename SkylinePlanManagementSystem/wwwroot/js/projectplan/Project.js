document.querySelectorAll(".btn-edit").forEach(btn => {
    btn.addEventListener("click", function () {
        let tr = this.closest("tr");

        tr.querySelectorAll(".view-mode").forEach(x => x.classList.add("d-none"));
        tr.querySelectorAll(".edit-mode").forEach(x => x.classList.remove("d-none"));

        tr.querySelector(".btn-edit").classList.add("d-none");
        tr.querySelector(".btn-save").classList.remove("d-none");
        tr.querySelector(".btn-cancel").classList.remove("d-none");
    });
});

document.querySelectorAll(".btn-cancel").forEach(btn => {
    btn.addEventListener("click", function () {
        let tr = this.closest("tr");

        tr.querySelectorAll(".view-mode").forEach(x => x.classList.remove("d-none"));
        tr.querySelectorAll(".edit-mode").forEach(x => x.classList.add("d-none"));

        tr.querySelector(".btn-edit").classList.remove("d-none");
        tr.querySelector(".btn-save").classList.add("d-none");
        tr.querySelector(".btn-cancel").classList.add("d-none");
    });
});

document.querySelectorAll('.btn-subnode-toggle').forEach(btn => {
    btn.addEventListener('click', function () {
        const target = document.querySelector(this.dataset.target);
        if (target) {
            target.classList.toggle('d-none');
        }
    });
});
