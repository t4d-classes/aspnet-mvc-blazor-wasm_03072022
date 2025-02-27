﻿using AutoMapper;

using ToolsApp.Core.Interfaces.Data;
using ToolsApp.Core.Interfaces.Models;

using CarModel = ToolsApp.Shared.Models.Car;
using CarDataModel = ToolsApp.Data.Models.Car;

namespace ToolsApp.Data;

public class CarsInMemoryData: ICarsData
{
  private IMapper _mapper;

  private List<CarDataModel> _cars = new List<CarDataModel>()
    {
      new() { Id=1, Make="Ford", Model="Fusion Hybrid", Year=2020, Color="blue", Price=45000 },
      new() { Id=2, Make="Tesla", Model="S", Year=2021, Color="red", Price=120000 },
    };

  public CarsInMemoryData()
  {
    var mapperConfig = new MapperConfiguration(config =>
    {
      config.CreateMap<INewCar, CarDataModel>();
      config.CreateMap<CarDataModel, CarModel>().ReverseMap();
    });

    _mapper = mapperConfig.CreateMapper();
  }

  public Task<IEnumerable<ICar>> All()
  {
    return Task.FromResult(_cars
      .Select(c => _mapper.Map<CarDataModel, CarModel>(c))
      .AsEnumerable<ICar>());
  }

  public Task<ICar> Append(INewCar car)
  {
    var newCarDataModel = _mapper.Map<CarDataModel>(car);
    newCarDataModel.Id = _cars.Any() ? _cars.Max(c => c.Id) + 1 : 1;

    _cars.Add(newCarDataModel);

    return Task.FromResult(
      _mapper.Map<CarDataModel, CarModel>(newCarDataModel) as ICar
    );
  }

  public Task<ICar?> One(int carId)
  {
    return Task.FromResult(_cars
      .Where(c => c.Id == carId)
      .Select(c => _mapper.Map<CarDataModel, CarModel>(c))
      .Cast<ICar>()
      .SingleOrDefault());
  }

  public Task Remove(int carId)
  {
    var carIndex = _cars.FindIndex(c => c.Id == carId);

    if (carIndex == -1)
    {
      throw new IndexOutOfRangeException("Car not found");
    }

    _cars.RemoveAt(carIndex);

    return Task.CompletedTask;
  }

  public Task Replace(ICar car)
  {
    var carDataModel = _mapper.Map<CarDataModel>(car);

    var carIndex = _cars.FindIndex(c => c.Id == carDataModel.Id);

    if (carIndex == -1) {
      throw new IndexOutOfRangeException("Car not found");
    }

    _cars[carIndex] = carDataModel;

    return Task.CompletedTask;
  }
}
