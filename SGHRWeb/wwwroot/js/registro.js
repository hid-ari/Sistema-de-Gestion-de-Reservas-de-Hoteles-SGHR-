// Script para mejorar la validación del formulario de registro

$(document).ready(function () {
    // Aplicar clase de error a inputs inválidos en tiempo real
    $('input').on('blur', function () {
        if (!$(this).valid()) {
            $(this).addClass('input-validation-error');
        } else {
            $(this).removeClass('input-validation-error');
        }
    });

    // Remover clase de error cuando el usuario empieza a escribir
    $('input').on('input', function () {
        if ($(this).hasClass('input-validation-error') && $(this).valid()) {
            $(this).removeClass('input-validation-error');
        }
    });

    // Validar antes de enviar el formulario
    $('#registroForm').on('submit', function (e) {
        var isValid = $(this).valid();
        
        if (!isValid) {
            // Agregar clase de error a todos los campos inválidos
            $(this).find('.input-validation-error').each(function () {
                $(this).addClass('input-validation-error');
            });
            
            // Hacer scroll al primer error
            var firstError = $('.input-validation-error:first');
            if (firstError.length > 0) {
                $('html, body').animate({
                    scrollTop: firstError.offset().top - 100
                }, 500);
            }
        }
    });
});
