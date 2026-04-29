function guardarEdicion()
{

    let idPedido = document.getElementById("idPedido").value;
    let idCliente = document.getElementById("clienteSelect").value;

    let detalles = [];

    document.querySelectorAll(".cantidad").forEach(input => {
    detalles.push({
    idProducto: parseInt(input.dataset.idproducto),
            cantidad: parseInt(input.value),
            precio: 0
        });
});

fetch('/Pedido/Editar', {
method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
    idPedido: parseInt(idPedido),
            idCliente: parseInt(idCliente),
            detalles: detalles
        })
    })
    .then(res => {
         if (!res.ok)
         {
             return res.text().then(text => { throw new Error(text); });
         }
         return res.json();
     })
    .then(res => {
        if (res.ok)
        {
            alert("Pedido actualizado correctamente");
            window.location.href = "/Pedido/Index";
        }
        else
        {
            alert("Stock Insuficiente");
        }
    })
    .catch(error => {
        console.error("ERROR:", error);
        alert("Error en el servidor: " + error.message);
    });
}