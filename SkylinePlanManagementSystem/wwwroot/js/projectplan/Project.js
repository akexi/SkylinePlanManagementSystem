
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
        if (target) {
            const isHidden = target.classList.toggle('d-none');
            toggleBtn.classList.toggle("is-expanded", !isHidden);
        }
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
        const token = addForm.querySelector('input[name="__RequestVerificationToken"]')?.value || "";

        if (!tbody) {
            const table = document.createElement("table");
            table.className = "table table-sm table-bordered mb-0 subnode-table";
            table.innerHTML = `<thead><tr><th style="width:80px;">ID</th><th>子节点名称</th><th style="width:160px;">计划时间</th><th style="width:220px;">操作</th></tr></thead><tbody></tbody>`;
            const anchor = box.querySelector(".subnode-table") || box.querySelector(".subnode-empty");
            if (anchor) {
                anchor.replaceWith(table);
            } else {
                box.appendChild(table);
            }
        }

        if (empty) empty.remove();
        const targetTbody = box.querySelector(".subnode-table tbody");
        const row = document.createElement("tr");
        row.className = "subnode-item-row";
        row.innerHTML = `
            <form action="/ProjectPlan/UpdateSubNode" method="post" class="subnode-edit-form">
                <input name="__RequestVerificationToken" type="hidden" value="${token}" />
                <input type="hidden" name="ProjectId" value="${addForm.querySelector('input[name="ProjectId"]').value}" />
                <input type="hidden" name="ProjectNodeId" value="${addForm.querySelector('input[name="ProjectNodeId"]').value}" />
                <input type="hidden" name="ProjectSubNodeId" value="${data.subNodeId}" />
                <td>${data.subNodeId}</td>
                <td><span class="view-mode">${data.title}</span><input type="text" name="Title" value="${data.title}" class="form-control form-control-sm edit-mode d-none" /></td>
                <td><span class="view-mode">${data.planTime || "-"}</span><input type="date" name="PlanTime" value="${data.planTime || ""}" class="form-control form-control-sm edit-mode d-none" /></td>
                <td><button type="button" class="btn btn-sm btn-outline-secondary btn-edit">编辑</button><button type="submit" class="btn btn-sm btn-outline-primary d-none btn-save">保存</button><button type="button" class="btn btn-sm btn-outline-secondary d-none btn-cancel">取消</button><button type="submit" formaction="/ProjectPlan/DeleteSubNode" name="projectSubNodeId" value="${data.subNodeId}" class="btn btn-sm btn-outline-danger" onclick="return confirm('确定删除子节点【${data.title}】吗？');">删除</button></td>
            </form>`;
        targetTbody.appendChild(row);
        addForm.reset();
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
