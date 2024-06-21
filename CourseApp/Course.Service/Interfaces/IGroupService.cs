using System;
using Course.Service.Dtos;
using Course.Service.Dtos.GroupDtos;
using Course.Service.Dtos.StudentDtos;

namespace Course.Service.Interfaces
{
	public interface IGroupService
	{
        int Create(GroupCreateDto createDto);
        PaginatedList<GroupGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<GroupGetDto> GetAll(string? search = null);
        GroupDetailsDto GetById(int id);
        void Update(int id, GroupUpdateDto updateDto);
        void Delete(int id);
    }
}

