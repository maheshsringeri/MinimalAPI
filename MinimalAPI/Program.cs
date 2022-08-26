using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using MinimalAPI;
using MinimalAPI.Data;
using MinimalAPI.Data.DTO;
using MinimalAPI.Models;
using MinimalAPI.Models.DTO;
using System.Diagnostics.CodeAnalysis;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    APIResponse response = new();
    _logger.Log(LogLevel.Information, "Getting all coupens");

    response.Result = CouponStore.couponList;
    response.IsSuccess = true;
    response.statusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupons").Produces<APIResponse>(200);

app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new();
    response.Result = CouponStore.couponList.FirstOrDefault(q => q.Id == id);
    response.IsSuccess = true;
    response.statusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200);

app.MapPost("/api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO couponCreateDTO) =>
{
    APIResponse response = new() { IsSuccess = false, statusCode = HttpStatusCode.BadRequest };


    var validationResult = await _validation.ValidateAsync(couponCreateDTO);

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    if (CouponStore.couponList.FirstOrDefault(q => q.Name.ToLower() == couponCreateDTO.Name.ToLower()) != null)
    {
        response.ErrorMessages.Add("Coupon name already exists");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponCreateDTO);

    coupon.Id = CouponStore.couponList.OrderByDescending(q => q.Id).FirstOrDefault().Id + 1;

    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    response.Result = couponDTO;
    response.IsSuccess = true;
    response.statusCode = HttpStatusCode.Created;
    return Results.Ok(response);

    // return Results.Created($"/api/coupon/{coupon.Id}", coupon);
    //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);

}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

app.MapPut("/api/coupon", async (IMapper _mapper, IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO couponUpdateDTO) =>
{
    APIResponse response = new() { IsSuccess = false, statusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(couponUpdateDTO);

    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(q => q.Id == couponUpdateDTO.Id);
    couponFromStore.Name = couponUpdateDTO.Name;
    couponFromStore.IsActive = couponUpdateDTO.IsActive;
    couponFromStore.Percent = couponUpdateDTO.Percent;
    couponFromStore.LastUpdated = DateTime.Now;

    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
    response.IsSuccess = true;
    response.statusCode = HttpStatusCode.OK;

    return Results.Ok(response);


}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);


app.MapDelete("/api/coupon{id:int}", (int id) =>
{
    APIResponse response = new APIResponse() { IsSuccess = false, statusCode = HttpStatusCode.BadRequest };

    var coupon = CouponStore.couponList.FirstOrDefault(q => q.Id == id);

    if (coupon == null)
    {
        response.ErrorMessages.Add("Invalid Id");
        return Results.BadRequest(response);
    }


    CouponStore.couponList.Remove(coupon);
    response.IsSuccess = true;
    response.statusCode = HttpStatusCode.NoContent;
    return Results.Ok(response);

}).WithName("DeleteCoupon");


app.UseHttpsRedirection();

app.Run();
