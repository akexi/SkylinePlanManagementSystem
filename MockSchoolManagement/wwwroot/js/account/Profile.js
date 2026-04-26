(function () {
    const emailInput = document.getElementById('Email');
    const phoneInput = document.getElementById('PhoneNumber');

    const editEmailBtn = document.getElementById('editEmailBtn');
    const cancelEmailBtn = document.getElementById('cancelEmailBtn');

    const editPhoneBtn = document.getElementById('editPhoneBtn');
    const cancelPhoneBtn = document.getElementById('cancelPhoneBtn');

    const saveRow = document.getElementById('saveRow');
    const cancelAllBtn = document.getElementById('cancelAllBtn');

    const originalEmail = emailInput.value;
    const originalPhone = phoneInput.value;

    function showSaveRow() {
        saveRow.classList.remove('d-none');
    }
    function hideSaveRowIfNoEdit() {
        if (emailInput.readOnly && phoneInput.readOnly) {
            saveRow.classList.add('d-none');
        }
    }

    // Email edit
    editEmailBtn.addEventListener('click', function () {
        emailInput.readOnly = false;
        emailInput.focus();
        editEmailBtn.classList.add('d-none');
        cancelEmailBtn.classList.remove('d-none');
        showSaveRow();
    });
    cancelEmailBtn.addEventListener('click', function () {
        emailInput.value = originalEmail;
        emailInput.readOnly = true;
        cancelEmailBtn.classList.add('d-none');
        editEmailBtn.classList.remove('d-none');
        hideSaveRowIfNoEdit();
    });

    // Phone edit
    editPhoneBtn.addEventListener('click', function () {
        phoneInput.readOnly = false;
        phoneInput.focus();
        editPhoneBtn.classList.add('d-none');
        cancelPhoneBtn.classList.remove('d-none');
        showSaveRow();
    });
    cancelPhoneBtn.addEventListener('click', function () {
        phoneInput.value = originalPhone;
        phoneInput.readOnly = true;
        cancelPhoneBtn.classList.add('d-none');
        editPhoneBtn.classList.remove('d-none');
        hideSaveRowIfNoEdit();
    });

    // 全部取消（表单级）
    cancelAllBtn.addEventListener('click', function () {
        // 恢复原值并禁用
        emailInput.value = originalEmail;
        phoneInput.value = originalPhone;
        emailInput.readOnly = true;
        phoneInput.readOnly = true;

        // 按钮状态
        cancelEmailBtn.classList.add('d-none');
        editEmailBtn.classList.remove('d-none');
        cancelPhoneBtn.classList.add('d-none');
        editPhoneBtn.classList.remove('d-none');

        hideSaveRowIfNoEdit();
    });

    // 页面卸载或提交时，不做额外处理；提交后服务器端刷新模型和视图。
})();
