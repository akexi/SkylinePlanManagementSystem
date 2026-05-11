
document.addEventListener("click", function (e) {
    const editBtn = e.target.closest(".btn-edit");
    const cancelBtn = e.target.closest(".btn-cancel");
    const toggleBtn = e.target.closest('.btn-subnode-toggle');

    if (editBtn) {
        const tr = editBtn.closest("tr");
        tr.querySelectorAll(".view-mode").forEach(x => x.classList.add("d-none"));
        tr.querySelectorAll(".edit-mode").forEach(x => x.classList.remove("d-none"));
        tr.querySelector(".btn-edit")?.classList.add("d-none");
        tr.querySelector(".btn-save")?.classList.remove("d-none");
        tr.querySelector(".btn-cancel")?.classList.remove("d-none");
    }

    if (cancelBtn) {
        const tr = cancelBtn.closest("tr");
        tr.querySelectorAll(".view-mode").forEach(x => x.classList.remove("d-none"));
        tr.querySelectorAll(".edit-mode").forEach(x => x.classList.add("d-none"));
        tr.querySelector(".btn-edit")?.classList.remove("d-none");
        tr.querySelector(".btn-save")?.classList.add("d-none");
        tr.querySelector(".btn-cancel")?.classList.add("d-none");
    }

            
    if (toggleBtn) {
        const target = document.querySelector(toggleBtn.dataset.target);
        if (target) target.classList.toggle('d-none');
    }
});

async function submitAjaxForm(form) {
    const res = await fetch(form.action, {
        method: 'POST',
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        body: new FormData(form)
    });
    return await res.json();
}

document.addEventListener("submit", async function (e) {
    const addForm = e.target.closest(".subnode-add-form");
    const editForm = e.target.closest(".subnode-edit-form");
    if (!addForm && !editForm) return;

    e.preventDefault();
    const form = e.target;
    const submitter = e.submitter;
    if (submitter?.getAttribute("formaction")) form.action = submitter.getAttribute("formaction");
    const data = await submitAjaxForm(form);
    if (!data.success) return alert(data.message || "操作失败");

    if (addForm) {
        const box = addForm.closest(".subnode-box");
        const tbody = box.querySelector(".subnode-table tbody");
        const empty = box.querySelector(".subnode-empty");
        if (empty) empty.remove();
        if (!tbody) { location.reload(); return; }
        const row = document.createElement("tr");
        row.className = "subnode-item-row";
        row.innerHTML = `<form asp-action="UpdateSubNode"></form>`;
        location.reload();
        return;
    }

    if (editForm && form.action.includes("DeleteSubNode")) {
        editForm.closest("tr")?.remove();
        return;
    }

    if (editForm) {
        const tr = editForm.closest("tr");
        tr.querySelector('span.view-mode').textContent = data.title;
        tr.querySelectorAll('span.view-mode')[1].textContent = data.planTime || '-';
        tr.querySelector(".btn-cancel")?.click();
    }
});
