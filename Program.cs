using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

// Перечисления
public enum DeviceType
{
    LIGHT,
    THERMOSTAT,
    SECURITY_CAMERA,
    SPEAKER,
    LOCK,
    SENSOR,
    PLUG
}

public enum RoomType
{
    LIVING_ROOM,
    BEDROOM,
    KITCHEN,
    BATHROOM,
    GARAGE,
    GARDEN
}

public enum Protocol
{
    WIFI,
    BLUETOOTH,
    ZIGBEE,
    Z_WAVE,
    THREAD
}

public enum Country
{
    USA,
    CHINA,
    GERMANY,
    SOUTH_KOREA,
    JAPAN,
    SWEDEN
}

// Структура для мощности
public struct Power
{
    private double _value;
    
    public double Value
    {
        get => _value;
        set
        {
            if (value < 0)
                throw new ArgumentException("Потребляемая мощность не может быть отрицательной");
            _value = value;
        }
    }
    
    public Power(double value)
    {
        if (value < 0)
            throw new ArgumentException("Потребляемая мощность не может быть отрицательной");
        _value = value;
    }
    
    public override string ToString() => $"{_value:F2}W";
}

// Класс производителя
public class Company
{
    public string Name { get; set; }
    public Country Country { get; set; }
    public int FoundationYear { get; set; }
    public int EmployeeCount { get; set; }
    public string Website { get; set; }
    
    public Company()
    {
        Name = "Unknown";
        Website = "";
    }
    
    public override string ToString()
    {
        return $"{Name} ({Country}), основана в {FoundationYear}, сотрудников: {EmployeeCount}";
    }
}

// Основной класс устройства умного дома
public class SmartHomeDevice : IEquatable<SmartHomeDevice>
{
    private static int _nextId = 1;
    
    // Автоматически генерируемый ID
    public int Id { get; set; }
    
    public string Name { get; set; }
    public Company Manufacturer { get; set; }
    public DeviceType DeviceType { get; set; }
    public RoomType Room { get; set; }
    public Power PowerConsumption { get; set; }
    public decimal Price { get; set; }
    public int WarrantyYears { get; set; }
    public Protocol ConnectionProtocol { get; set; }
    public DateTime InstallationDate { get; set; }
    public bool IsActive { get; set; }
    public List<string> Features { get; set; }
    
    // Автоматически генерируемое поле
    public DateTime CreatedAt { get; set; }
    
    public SmartHomeDevice()
    {
        Id = _nextId++;
        CreatedAt = DateTime.Now;
        Features = new List<string>();
        Manufacturer = new Company();
        Name = "Unnamed Device";
    }
    
    public bool Equals(SmartHomeDevice other)
    {
        if (other is null) return false;
        return Id == other.Id;
    }
    
    public override bool Equals(object obj) => Equals(obj as SmartHomeDevice);
    
    public override int GetHashCode() => Id.GetHashCode();
    
    public override string ToString()
    {
        return $"ID: {Id}, {Name} ({DeviceType}) - {Manufacturer.Name}\n" +
               $"Комната: {Room}, Мощность: {PowerConsumption}, Цена: {Price:C}\n" +
               $"Протокол: {ConnectionProtocol}, Установлен: {InstallationDate:dd.MM.yyyy}\n" +
               $"Активно: {IsActive}, Гарантия: {WarrantyYears} лет\n" +
               $"Функции: {string.Join(", ", Features)}\n" +
               $"Добавлено: {CreatedAt:dd.MM.yyyy HH:mm}";
    }
}

// Основной класс управления
public class SmartHomeManager
{
    private HashSet<SmartHomeDevice> _devices;
    private string _dataFile;
    
    public SmartHomeManager(string dataFile)
    {
        _devices = new HashSet<SmartHomeDevice>();
        _dataFile = dataFile;
        LoadData();
    }
    
    // Загрузка данных из XML
    private void LoadData()
    {
        if (!File.Exists(_dataFile))
        {
            Console.WriteLine($"Файл {_dataFile} не найден. Будет создана пустая коллекция.");
            return;
        }
        
        try
        {
            var serializer = new XmlSerializer(typeof(List<SmartHomeDevice>));
            using (var reader = new StreamReader(_dataFile))
            {
                var devices = (List<SmartHomeDevice>)serializer.Deserialize(reader);
                _devices = new HashSet<SmartHomeDevice>(devices);
            }
            Console.WriteLine($"Загружено {_devices.Count} устройств из {_dataFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
    }
    
    // Сохранение данных в XML
    public void SaveData()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(List<SmartHomeDevice>));
            using (var writer = new StreamWriter(_dataFile))
            {
                serializer.Serialize(writer, _devices.ToList());
            }
            Console.WriteLine($"Данные сохранены в {_dataFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения данных: {ex.Message}");
        }
    }
    
    // Вывод справки
    public void Help()
    {
        Console.WriteLine("Доступные команды:");
        Console.WriteLine("  help - вывести справку");
        Console.WriteLine("  info - информация о коллекции");
        Console.WriteLine("  show - показать все устройства");
        Console.WriteLine("  insert - добавить новое устройство");
        Console.WriteLine("  update id - обновить устройство по ID");
        Console.WriteLine("  remove_key id - удалить устройство по ID");
        Console.WriteLine("  clear - очистить коллекцию");
        Console.WriteLine("  save - сохранить коллекцию");
        Console.WriteLine("  execute_script file_name - выполнить скрипт");
        Console.WriteLine("  exit - выход");
        Console.WriteLine("Дополнительные команды:");
        Console.WriteLine("  remove_greater_key id - удалить устройства с ID больше указанного");
        Console.WriteLine("  remove_lower_key id - удалить устройства с ID меньше указанного");
        Console.WriteLine("  print_unique_field - вывести уникальные значения поля");
    }
    
    // Информация о коллекции
    public void Info()
    {
        Console.WriteLine($"Тип коллекции: HashSet<SmartHomeDevice>");
        Console.WriteLine($"Количество элементов: {_devices.Count}");
        Console.WriteLine($"Дата инициализации: {DateTime.Now}");
    }
    
    // Показать все устройства
    public void Show()
    {
        if (_devices.Count == 0)
        {
            Console.WriteLine("Коллекция пуста");
            return;
        }
        
        foreach (var device in _devices.OrderBy(d => d.Id))
        {
            Console.WriteLine(device);
            Console.WriteLine("---");
        }
    }
    
    // Добавление устройства
    public void Insert()
    {
        try
        {
            var device = ReadDeviceFromConsole();
            if (_devices.Add(device))
            {
                Console.WriteLine($"Устройство добавлено с ID: {device.Id}");
            }
            else
            {
                Console.WriteLine("Устройство с таким ID уже существует");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка добавления: {ex.Message}");
        }
    }
    
    // Обновление устройства
    public void Update(int id)
    {
        var existingDevice = _devices.FirstOrDefault(d => d.Id == id);
        if (existingDevice == null)
        {
            Console.WriteLine($"Устройство с ID {id} не найдено");
            return;
        }
        
        try
        {
            _devices.Remove(existingDevice);
            var updatedDevice = ReadDeviceFromConsole();
            updatedDevice.Id = id; // Сохраняем оригинальный ID
            _devices.Add(updatedDevice);
            Console.WriteLine($"Устройство с ID {id} обновлено");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обновления: {ex.Message}");
        }
    }
    
    // Удаление устройства
    public void RemoveKey(int id)
    {
        var device = _devices.FirstOrDefault(d => d.Id == id);
        if (device != null && _devices.Remove(device))
        {
            Console.WriteLine($"Устройство с ID {id} удалено");
        }
        else
        {
            Console.WriteLine($"Устройство с ID {id} не найдено");
        }
    }
    
    // Очистка коллекции
    public void Clear()
    {
        _devices.Clear();
        Console.WriteLine("Коллекция очищена");
    }
    
    // Удаление устройств с ID больше указанного
    public void RemoveGreaterKey(int id)
    {
        var removedCount = _devices.RemoveWhere(d => d.Id > id);
        Console.WriteLine($"Удалено {removedCount} устройств с ID больше {id}");
    }
    
    // Удаление устройств с ID меньше указанного
    public void RemoveLowerKey(int id)
    {
        var removedCount = _devices.RemoveWhere(d => d.Id < id);
        Console.WriteLine($"Удалено {removedCount} устройств с ID меньше {id}");
    }
    
    // Вывод уникальных значений поля
    public void PrintUniqueField(string fieldName)
    {
        switch (fieldName.ToLower())
        {
            case "devicetype":
                var uniqueTypes = _devices.Select(d => d.DeviceType).Distinct();
                Console.WriteLine("Уникальные типы устройств:");
                foreach (var type in uniqueTypes)
                    Console.WriteLine($"  {type}");
                break;
                
            case "room":
                var uniqueRooms = _devices.Select(d => d.Room).Distinct();
                Console.WriteLine("Уникальные комнаты:");
                foreach (var room in uniqueRooms)
                    Console.WriteLine($"  {room}");
                break;
                
            case "protocol":
                var uniqueProtocols = _devices.Select(d => d.ConnectionProtocol).Distinct();
                Console.WriteLine("Уникальные протоколы:");
                foreach (var protocol in uniqueProtocols)
                    Console.WriteLine($"  {protocol}");
                break;
                
            case "manufacturer":
                var uniqueManufacturers = _devices.Select(d => d.Manufacturer.Name).Distinct();
                Console.WriteLine("Уникальные производители:");
                foreach (var manufacturer in uniqueManufacturers)
                    Console.WriteLine($"  {manufacturer}");
                break;
                
            default:
                Console.WriteLine($"Поле {fieldName} не поддерживается для вывода уникальных значений");
                Console.WriteLine("Доступные поля: DeviceType, Room, Protocol, Manufacturer");
                break;
        }
    }
    
    // Чтение устройства из консоли
    private SmartHomeDevice ReadDeviceFromConsole()
    {
        var device = new SmartHomeDevice();
        
        Console.Write("Название устройства: ");
        device.Name = Console.ReadLine();
        
        Console.WriteLine("Производитель:");
        device.Manufacturer = ReadCompanyFromConsole();
        
        Console.WriteLine("Тип устройства (" + string.Join(", ", Enum.GetNames(typeof(DeviceType))) + "): ");
        device.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), Console.ReadLine(), true);
        
        Console.WriteLine("Комната (" + string.Join(", ", Enum.GetNames(typeof(RoomType))) + "): ");
        device.Room = (RoomType)Enum.Parse(typeof(RoomType), Console.ReadLine(), true);
        
        Console.Write("Потребляемая мощность (Вт): ");
        device.PowerConsumption = new Power(double.Parse(Console.ReadLine()));
        
        Console.Write("Цена: ");
        device.Price = decimal.Parse(Console.ReadLine());
        
        Console.Write("Гарантия (лет): ");
        device.WarrantyYears = int.Parse(Console.ReadLine());
        
        Console.WriteLine("Протокол связи (" + string.Join(", ", Enum.GetNames(typeof(Protocol))) + "): ");
        device.ConnectionProtocol = (Protocol)Enum.Parse(typeof(Protocol), Console.ReadLine(), true);
        
        Console.Write("Дата установки (гггг-мм-дд): ");
        device.InstallationDate = DateTime.Parse(Console.ReadLine());
        
        Console.Write("Активно (true/false): ");
        device.IsActive = bool.Parse(Console.ReadLine());
        
        Console.WriteLine("Введите функции устройства (пустая строка для завершения):");
        device.Features.Clear();
        while (true)
        {
            var feature = Console.ReadLine();
            if (string.IsNullOrEmpty(feature)) break;
            device.Features.Add(feature);
        }
        
        return device;
    }
    
    // Чтение компании из консоли
    private Company ReadCompanyFromConsole()
    {
        var company = new Company();
        
        Console.Write("  Название компании: ");
        company.Name = Console.ReadLine();
        
        Console.WriteLine("  Страна (" + string.Join(", ", Enum.GetNames(typeof(Country))) + "): ");
        company.Country = (Country)Enum.Parse(typeof(Country), Console.ReadLine(), true);
        
        Console.Write("  Год основания: ");
        company.FoundationYear = int.Parse(Console.ReadLine());
        
        Console.Write("  Количество сотрудников: ");
        company.EmployeeCount = int.Parse(Console.ReadLine());
        
        Console.Write("  Веб-сайт: ");
        company.Website = Console.ReadLine();
        
        return company;
    }
    
    // Выполнение скрипта
    public void ExecuteScript(string fileName)
    {
        if (!File.Exists(fileName))
        {
            Console.WriteLine($"Файл скрипта {fileName} не найден");
            return;
        }
        
        try
        {
            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                
                Console.WriteLine($"> {line}");
                ProcessCommand(line);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения скрипта: {ex.Message}");
        }
    }
    
    // Обработка команд
    public void ProcessCommand(string commandLine)
    {
        var parts = commandLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;
        
        var command = parts[0].ToLower();
        
        try
        {
            switch (command)
            {
                case "help":
                    Help();
                    break;
                    
                case "info":
                    Info();
                    break;
                    
                case "show":
                    Show();
                    break;
                    
                case "insert":
                    Insert();
                    break;
                    
                case "update":
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int updateId))
                        Update(updateId);
                    else
                        Console.WriteLine("Использование: update id");
                    break;
                    
                case "remove_key":
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int removeId))
                        RemoveKey(removeId);
                    else
                        Console.WriteLine("Использование: remove_key id");
                    break;
                    
                case "clear":
                    Clear();
                    break;
                    
                case "save":
                    SaveData();
                    break;
                    
                case "execute_script":
                    if (parts.Length >= 2)
                        ExecuteScript(parts[1]);
                    else
                        Console.WriteLine("Использование: execute_script file_name");
                    break;
                    
                case "remove_greater_key":
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int greaterId))
                        RemoveGreaterKey(greaterId);
                    else
                        Console.WriteLine("Использование: remove_greater_key id");
                    break;
                    
                case "remove_lower_key":
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int lowerId))
                        RemoveLowerKey(lowerId);
                    else
                        Console.WriteLine("Использование: remove_lower_key id");
                    break;
                    
                case "print_unique_field":
                    if (parts.Length >= 2)
                        PrintUniqueField(parts[1]);
                    else
                        Console.WriteLine("Использование: print_unique_field field_name");
                    break;
                    
                case "exit":
                    Console.WriteLine("Выход из программы...");
                    Environment.Exit(0);
                    break;
                    
                default:
                    Console.WriteLine($"Неизвестная команда: {command}");
                    Console.WriteLine("Введите 'help' для списка команд");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения команды: {ex.Message}");
        }
    }
    
    // Основной цикл программы
    public void Run()
    {
        Console.WriteLine("Управление умным домом - версия 1.0");
        Console.WriteLine("Введите 'help' для списка команд");
        
        while (true)
        {
            Console.Write("> ");
            var command = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(command)) continue;
            
            ProcessCommand(command);
        }
    }
}

// Главный класс программы
class Program
{
    static void Main(string[] args)
    {
        string dataFile;
        
        // Определение файла данных
        if (args.Length > 0)
        {
            dataFile = args[0];
        }
        else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SMARTHOME_DATA")))
        {
            dataFile = Environment.GetEnvironmentVariable("SMARTHOME_DATA");
        }
        else
        {
            Console.Write("Введите имя файла данных: ");
            dataFile = Console.ReadLine();
        }
        
        if (string.IsNullOrEmpty(dataFile))
        {
            dataFile = "devices.xml";
        }
        
        try
        {
            var manager = new SmartHomeManager(dataFile);
            manager.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}