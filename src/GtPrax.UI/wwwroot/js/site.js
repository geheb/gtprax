function createToast(text, colorClass = 'is-primary') {
    const id = '_' + Math.random().toString(36).substring(2, 9);
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

function sleep(time) {
  return new Promise((resolve) => setTimeout(resolve, time));
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
            action: function (target, data) { return data; },
        },
        confirm: {
            dataurl: '',
            geturl: function (target, url) { return url; },
            action: function () { }
        },
    };
    const params = { ...defaults, ...args };
    const modal = $(params.id);

    modal.on('modal:open', function (e) {
        if (params.init.datainfo) {
            const info = e.relatedTarget.dataset[params.init.datainfo];
            params.init.action(e.target, info);
        }
        const confirmButton = $(e.target).find(".confirm");
        const loading = $(e.target).find('.loading-value');
        const closeButton = $(e.target).find(".close-modal:first");

        if (params.load.dataurl) {
            loading.removeClass('is-hidden');
            const url = e.relatedTarget.dataset[params.load.dataurl];
            $.post(url, params.token).done(function (response) {
                if (response.success) {
                    loading.addClass('is-hidden');
                    params.load.action(e.target, response.data);
                    confirmButton.attr("disabled", false);
                } else {
                    createToast(response.error, 'is-danger');
                }
            });
        } else {
          confirmButton.attr("disabled", false);
        }

        if (params.confirm.dataurl) {
            const dataurl = e.relatedTarget.dataset[params.confirm.dataurl];
            
            confirmButton.on('click', function (evClick) {
                evClick.preventDefault();
                confirmButton.addClass("is-loading");
                const url = params.confirm.geturl ? params.confirm.geturl(e.target, dataurl) : dataurl;

                $.post(url, params.token).done(function (response) {
                    if (response.success) {
                      params.confirm.action();
                      sleep(1000).then(() => closeButton.trigger("click"));
                    } else {
                        createToast(response.error, 'is-danger');
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

$(function () {
    $(".navbar-burger").on('click', function () {
        $(".navbar-burger").toggleClass("is-active");
        $(".navbar-menu").toggleClass("is-active");
    });

    // should work for dynamic created elements also
    $("body").on("click", ".notification > button.delete", function () {
        $(this).parent().addClass('is-hidden').remove();
        return false;
    });

    $('.open-modal').on('click', openModal);
    $('.close-modal').on('click', closeModal);

    $('input[type=password]').on('input', function () {
        const eye = $(this).parent().find('.toggle-eye');
        eye?.css('visibility', $(this).val() ? 'visible' : 'hidden');
    });

    $('.toggle-eye > i').on('click', function () {
        const input = $(this).parent().parent().find('input');
        const isPassword = input.attr('type') === 'password';
        input.attr('type', isPassword ? 'text' : 'password');
        $(this).toggleClass("fa-eye fa-eye-slash");
    });

    $(".list-item-clickable").on('click', function (e) {
        e.stopPropagation();
        window.location = $(this).data("url");
    });
});
