document.addEventListener("click", function (e) {
    const editBtn = e.target.closest(".btn-edit");
    const cancelBtn = e.target.closest(".btn-cancel");
    const toggleBtn = e.target.closest('.btn-subnode-toggle');

    if (editBtn) {
        const tr = editBtn.closest("tr");
        tr.classList.add("is-editing");
        tr.querySelectorAll(".view-mode").forEach(x => x.classList.add("d-none"));
        tr.querySelectorAll(".edit-mode").forEach(x => x.classList.remove("d-none"));
        tr.querySelector(".btn-edit")?.classList.add("d-none");
        tr.querySelector(".btn-save")?.classList.remove("d-none");
        tr.querySelector(".btn-cancel")?.classList.remove("d-none");
    }

    if (cancelBtn) {
        const tr = cancelBtn.closest("tr");
        tr.classList.remove("is-editing");
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

function escapeHtml(str) {
    if (!str) return "";
    return String(str).replace(/[&<>"'`=\/]/g, function (s) {
        return ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#39;",
            "/": "&#x2F;",
            "`": "&#x60;",
            "=": "&#x3D;"
        })[s];
    });
}

function updateNodeProgressBar(nodeId, progress) {
    const progressBars = document.querySelectorAll(`.node-progress-bar-${nodeId}`);
    progressBars.forEach(bar => {
        const percentage = parseFloat(progress).toFixed(2);
        bar.style.width = `${percentage}%`;
        bar.setAttribute("aria-valuenow", percentage);
        bar.textContent = `${parseFloat(percentage)}%`;
    });
}

function updateProjectProgressBar(progress) {
    const bar = document.getElementById("project-overall-progress");
    if (bar) {
        const percentage = parseFloat(progress).toFixed(2);
        bar.style.width = `${percentage}%`;
        bar.setAttribute("aria-valuenow", percentage);
        bar.textContent = `${parseFloat(percentage)}%`;
    }
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
    if (!data || data.success === false) return alert(data?.message || "操作失败");

    // 新增子节点后在 DOM 中追加行（兼容新增返回的字段：subNodeId, title, planStartTime, planEndTime, progressStatus）
    if (addForm) {
        const box = addForm.closest(".subnode-box");
        const existingTable = box.querySelector(".subnode-table");
        const empty = box.querySelector(".subnode-empty");
        const token = addForm.querySelector('input[name="__RequestVerificationToken"]')?.value || "";

        if (!existingTable) {
            const table = document.createElement("table");
            table.className = "table table-sm table-bordered mb-0 subnode-table";
            table.innerHTML = `<thead><tr><th style="width:60px;">ID</th><th>二级节点名称</th><th>明细（三级节点）</th><th style="width:180px;">计划开始/完成</th><th style="width:120px;">状态</th><th style="width:180px;">备注</th><th style="width:200px;">操作</th></tr></thead><tbody></tbody>`;
            box.appendChild(table);
        }

        if (empty) empty.remove();
        const targetTbody = box.querySelector(".subnode-table tbody");
        const subNodeId = data.subNodeId;
        const title = escapeHtml(data.title);
        const detail = escapeHtml(data.detail || "-");
        const planStart = data.planStartTime || "";
        const planEnd = data.planEndTime || "";
        const progress = escapeHtml(data.progressStatus || "-");
        const projectId = addForm.querySelector('input[name="ProjectId"]').value;
        const projectNodeId = addForm.querySelector('input[name="ProjectNodeId"]').value;

        const row = document.createElement("tr");
        row.className = "subnode-item-row";
        row.dataset.subnodeId = subNodeId;
        row.innerHTML = `
            <td>
                ${escapeHtml(subNodeId)}
                <form id="subnode-edit-form-${subNodeId}" action="/ProjectPlan/UpdateSubNode" method="post" class="subnode-edit-form" style="display:none;">
                    <input name="__RequestVerificationToken" type="hidden" value="${escapeHtml(token)}" />
                    <input type="hidden" name="ProjectId" value="${escapeHtml(projectId)}" />
                    <input type="hidden" name="ProjectNodeId" value="${escapeHtml(projectNodeId)}" />
                    <input type="hidden" name="ProjectSubNodeId" value="${escapeHtml(subNodeId)}" />
                </form>
            </td>
            <td>
                <span class="view-mode">${title}</span>
                <input type="text" name="Title" value="${title}" form="subnode-edit-form-${subNodeId}" class="form-control form-control-sm edit-mode d-none" style="width:100%;margin:0;" />
            </td>
            <td>
                <span class="view-mode">${detail}</span>
                <input type="text" name="Detail" value="${escapeHtml(data.detail || "")}" form="subnode-edit-form-${subNodeId}" class="form-control form-control-sm edit-mode d-none" style="width:100%;margin:0;" />
            </td>
            <td>
                <span class="view-mode">${planStart || "-"}</span>
                <input type="date" name="PlanStartTime" value="${escapeHtml(planStart)}" form="subnode-edit-form-${subNodeId}" class="form-control form-control-sm edit-mode d-none" />
                <span class="view-mode"> / ${planEnd || "-"}</span>
                <input type="date" name="PlanEndTime" value="${escapeHtml(planEnd)}" form="subnode-edit-form-${subNodeId}" class="form-control form-control-sm edit-mode d-none" />
            </td>
            <td>
                <span class="view-mode">${progress}</span>
                <select name="ProgressStatus" form="subnode-edit-form-${subNodeId}" class="custom-select custom-select-sm edit-mode d-none">
                    <option${progress === "未开始" ? " selected" : ""}>未开始</option>
                    <option${progress === "进行中" ? " selected" : ""}>进行中</option>
                    <option${progress === "已完成" ? " selected" : ""}>已完成</option>
                    <option${progress === "已延期" ? " selected" : ""}>已延期</option>
                </select>
            </td>
            <td>
                <span class="view-mode">${escapeHtml(data.remark || "")}</span>
                <input type="text" name="Remark" value="${escapeHtml(data.remark || "")}" form="subnode-edit-form-${subNodeId}" class="form-control form-control-sm edit-mode d-none" style="width:100%;margin:0;" />
            </td>
            <td>
                <button type="button" class="btn btn-sm btn-outline-secondary btn-edit">编辑</button>
                <button type="submit" form="subnode-edit-form-${subNodeId}" class="btn btn-sm btn-outline-primary d-none btn-save">保存</button>
                <button type="button" class="btn btn-sm btn-outline-secondary d-none btn-cancel">取消</button>
                <button type="submit" form="subnode-edit-form-${subNodeId}" formaction="/ProjectPlan/DeleteSubNode" name="projectSubNodeId" value="${escapeHtml(subNodeId)}" class="btn btn-sm btn-outline-danger" onclick="return confirm('确定删除子节点【${title}】吗？');">删除</button>
            </td>`;
        targetTbody.appendChild(row);
        addForm.reset();

        if (data.nodeProgress !== undefined) {
            updateNodeProgressBar(projectNodeId, data.nodeProgress);
        }
        if (data.projectProgress !== undefined) {
            updateProjectProgressBar(data.projectProgress);
        }
        return;
    }

    // 编辑表单：删除或更新
    if (editForm && form.action.includes("DeleteSubNode")) {
        const projectNodeId = editForm.querySelector('input[name="ProjectNodeId"]').value;
        const subNodeId = editForm.querySelector('input[name="ProjectSubNodeId"]').value;
        const tr = document.querySelector(`.subnode-item-row[data-subnode-id="${subNodeId}"]`) || editForm.closest("tr");
        tr?.remove();

        if (data.nodeProgress !== undefined) {
            updateNodeProgressBar(projectNodeId, data.nodeProgress);
        }
        if (data.projectProgress !== undefined) {
            updateProjectProgressBar(data.projectProgress);
        }
        return;
    }

    if (editForm) {
        const projectNodeId = editForm.querySelector('input[name="ProjectNodeId"]').value;
        const subNodeId = editForm.querySelector('input[name="ProjectSubNodeId"]').value;
        const tr = document.querySelector(`.subnode-item-row[data-subnode-id="${subNodeId}"]`) || editForm.closest("tr");

        // 更新显示字段（title、start/end、progress, remark）
        const tds = tr.querySelectorAll("td");
        if (tds.length >= 6) {
            const titleSpan = tds[1].querySelector("span.view-mode");
            if (titleSpan) titleSpan.textContent = data.title || "";
            const detailSpan = tds[2].querySelector("span.view-mode");
            if (detailSpan) detailSpan.textContent = data.detail || "-";
            const timeSpans = tds[3].querySelectorAll("span.view-mode");
            if (timeSpans.length > 0) timeSpans[0].textContent = data.planStartTime || "-";
            if (timeSpans.length > 1) timeSpans[1].textContent = " / " + (data.planEndTime || "-");
            const progressSpan = tds[4].querySelector("span.view-mode");
            if (progressSpan) progressSpan.textContent = data.progressStatus || "-";
            const remarkSpan = tds[5].querySelector("span.view-mode");
            if (remarkSpan) remarkSpan.textContent = data.remark || "";
        }
        // 退出编辑态（触发取消逻辑）
        tr.querySelector(".btn-cancel")?.click();

        if (data.nodeProgress !== undefined) {
            updateNodeProgressBar(projectNodeId, data.nodeProgress);
        }
        if (data.projectProgress !== undefined) {
            updateProjectProgressBar(data.projectProgress);
        }
    }
});