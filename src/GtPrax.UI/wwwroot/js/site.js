function createToast(text, colorClass = 'is-primary') {
    const id = '_' + Math.random().toString(36).substr(2, 9);
    const html = `<div class="notification ${colorClass} has-fadein" id="${id}"><button class="delete"></button>${text}</div>`;
    $(".toast-container").append(html);
    const element = $(`#${id}`);
    setTimeout(function () {
        element.fadeOut();
        element.remove();
    }, 5000);
};

function openModal(event) {
    event.stopPropagation();
    event.preventDefault();
    const id = event.currentTarget.dataset.target;
    const modal = $(id);
    modal.toggleClass('is-active');
    $('html').toggleClass('is-clipped');
    const openEvent = jQuery.Event("modal:open");
    openEvent.relatedTarget = event.currentTarget;
    modal.trigger(openEvent);
};

function closeModal(event) {
    event.stopPropagation();
    const modal = $(this).closest(".modal");
    modal.toggleClass('is-active');
    $('html').toggleClass('is-clipped');
    modal.trigger("modal:close");
}

function handleModal(args) {
    const defaults = {
        id: '',
        token: '',
        init: {
            datainfo: '',
            action: function (target, data) { }
        },
        load: {
            dataurl: '',
            dataformat: function (data) { return data; },
            selector: '',
            toast: { failed: 'Fehler' }
        },
        confirm: {
            dataurl: '',
            pre: function (target, url) { return url; },
            post: function (data) { return true; },
            toast: { success: 'OK', failed: 'Fehler' }
        },
    };
    const params = { ...defaults, ...args };
    const modal = $(params.id);

    modal.on('modal:open', function (e) {
        if (params.init.datainfo) {
            const info = e.relatedTarget.dataset[params.init.datainfo];
            params.init.action(e.target, info);
        }
        const confirm = $(e.target).find(".confirm");
        const loading = $(e.target).find('.loading-value');

        if (params.load.dataurl) {
            loading.removeClass('is-hidden');
            const url = e.relatedTarget.dataset[params.load.dataurl];
            $.post(url, params.token).done(function (data) {
                if (data) {
                    loading.addClass('is-hidden');
                    const format = params.load.dataformat(data);
                    $(e.target).find(params.load.selector).text(format);
                    confirm.attr("disabled", false);
                } else {
                    createToast(params.load.toast.failed);
                }
            });
        } else {
            confirm.attr("disabled", false);
        }

        if (params.confirm.dataurl) {
            const dataurl = e.relatedTarget.dataset[params.confirm.dataurl];
            
            confirm.click(function (evClick) {
                evClick.preventDefault();
                confirm.addClass("is-loading");
                const url = params.confirm.pre ? params.confirm.pre(e.target, dataurl) : dataurl;

                $.post(url, params.token).done(function (data) {
                    if (data) {
                        const showToast = params.confirm.post(data);
                        if (showToast) {
                            createToast(params.confirm.toast.success);
                        }
                    } else {
                        createToast(params.confirm.toast.failed, 'is-danger');
                    }
                });
            });
        }
    });

    modal.on('modal:close', function (e) {
        $(e.target).find('.loading-value').addClass('is-hidden');
        const confirm = $(e.target).find(".confirm");
        if (confirm) {
            confirm.attr("disabled", true);
            confirm.removeClass("is-loading");
            confirm.off();
        }
    });
}

function sleep(time) {
    return new Promise((resolve) => setTimeout(resolve, time));
}


$(function () {
    $(".navbar-burger").click(function () {
        $(".navbar-burger").toggleClass("is-active");
        $(".navbar-menu").toggleClass("is-active");
    });

    // should work for dynamic created elements also
    $("body").on("click", ".notification > button.delete", function () {
        $(this).parent().addClass('is-hidden').remove();
        return false;
    });

    $(".copy-link").click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        const url = $(this).data('url');
        const tempInput = $("<input>");
        $("body").append(tempInput);
        tempInput.val(url).select();
        document.execCommand("copy");
        tempInput.remove();
        createToast("Link wurde in die Zwischenablage kopiert.");
    });

    $('.open-modal').click(openModal);
    $('.close-modal').click(closeModal);

    $('input[type=password]').on('input', function () {
        const eye = $(this).parent().find('.toggle-eye');
        eye?.css('visibility', $(this).val() ? 'visible' : 'hidden');
    });

    $('.toggle-eye > i').click(function () {
        const input = $(this).parent().parent().find('input');
        const isPassword = input.attr('type') === 'password';
        input.attr('type', isPassword ? 'text' : 'password');
        $(this).toggleClass("fa-eye fa-eye-slash");
    });

    $(".list-item-clickable").click(function (e) {
        e.stopPropagation();
        window.location = $(this).data("url");
    });
});